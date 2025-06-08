using System;
using System.Collections;
using System.Collections.Generic;
using CamLib;
using DG.Tweening;
using Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;

     public EdgeCollider2D MovementBoundaries;
     public Transform Background;

     public List<BackdropInstance> Backdrops;
     
     [Serializable]
     public class BackdropInstance
     {
         public Background Type; 
         public Parallax Image;
     }

     public List<EnemyController> Enemies = new();
     
     public LevelInfos Levels;
     [Space]
     public PulseText PulseTextPrefab;
     
     private Dictionary<Element, Color> ElementColors = new();
     
     private bool GameInProgress;
     private bool ChoosingNewUpgrade;
     private int Score;
     private bool InIntermission;
     
     [NonSerialized] public List<Upgrade> IntermissionUpgradeOptions = new();
     
     private List<Upgrade> UpgradePool = new()
     {
         Upgrade.EngineSpeed1,
         Upgrade.Laser1,
         Upgrade.Flamethrower1,
         Upgrade.Cryo1,
         Upgrade.Lightning1
     };
     
     public int GetScore => Score;

     

     protected override void Awake()
     {
         base.Awake();

         Player.gameObject.SetActive(false);
         
         ElementColors.Add(Element.Normal, Color.white);
         ElementColors.Add(Element.Fire, new Color(1f, 0.5f, 0.2f));
         ElementColors.Add(Element.Ice, Color.cyan);
         ElementColors.Add(Element.Electric, Color.yellow);
     }
    
     public void StartGame()
     {
         if (GameInProgress) return;
         
         IEnumerator GameTransition() 
         {
             HudManager.Instance.FadeIntoBlack(1);
    
             yield return new WaitForSeconds(1);

             HudManager.Instance.MainMenu.Canvas.enabled = false;
             HudManager.Instance.ActivateGameHud();
    
             Player.gameObject.SetActive(true);
             
             Background.Rotate(Vector3.forward, -90);
             
             HudManager.Instance.HideGameHud(false);
             
             HudManager.Instance.FadeOutOfBlack(1);
    
             yield return new WaitForSeconds(1);
    
             GameInProgress = true;


             yield return GameProgression();

         }
    
         StartCoroutine(GameTransition());
     }

     public void HandleIntermissionUpgrade()
     {
         ChoosingNewUpgrade = true;

         UpgradePool.Shuffle();
         
         for (int i = 0; i < 3; i++)
         {
             if (UpgradePool.Count <= i) break;
             
             IntermissionUpgradeOptions.Add(UpgradePool[i]);
         }

         HudManager.Instance.ShowUpgradeScreen(true);
     }

     public void HandleAddUpgrade(Upgrade upgrade)
     {
         IntermissionUpgradeOptions.Clear();
         UpgradePool.Remove(upgrade);
         
         Player.AddUpgrade(upgrade);

         ChoosingNewUpgrade = false;

         switch (upgrade)
         {
             case Upgrade.EngineSpeed1:
                 UpgradePool.Add(Upgrade.EngineSpeed2);
                 break;
             case Upgrade.EngineSpeed2:
                 UpgradePool.Add(Upgrade.EngineSpeed3);
                 break;
             case Upgrade.Laser1:
                 UpgradePool.Add(Upgrade.Laser2);
                 break;
             case Upgrade.Laser2:
                 UpgradePool.Add(Upgrade.Laser3);
                 UpgradePool.Add(Upgrade.Shotgun);
                 break;
             case Upgrade.Flamethrower1:
                 UpgradePool.Add(Upgrade.Flamethrower2);
                 break;
             case Upgrade.Flamethrower2:
                 UpgradePool.Add(Upgrade.Flamethrower3);
                 break;
             case Upgrade.Cryo1:
                 UpgradePool.Add(Upgrade.Cryo2);
                 break;
             case Upgrade.Cryo2:
                 UpgradePool.Add(Upgrade.Cryo3);
                 break;
             case Upgrade.Lightning1:
                 UpgradePool.Add(Upgrade.Lightning2);
                 break;
             case Upgrade.Lightning2:
                 UpgradePool.Add(Upgrade.Lightning3);
                 break;
         }
     }

     private IEnumerator GameProgression()
     {
         for (int i = 0; i < Levels.Infos.Length; i++)
         {
             LevelInfo level = Levels.Infos[i];
             Debug.Log($"startup level {level.LevelName}");
             LevelProgress = i;
             ResetDeaths();
             ResetTimeBonus();
             UpdateTotalProgress();

             HudManager.Instance.Toast(level.LevelName);

             SetABackdrop(level.BackgroundToShow);

             yield return EnemyDirector.Instance.SpawnWaves(level.Waves);

             yield return LevelTransition(level.LevelName, i == Levels.Infos.Length-1);
         }

         FinalRankingUI.Instance.Show(Score);
     }

     private void ResetDeaths()
     {
         DeathsThisLevel = 0;
         HudDeaths.Instance.SetDeaths(0);
     }

     public float LevelProgress;

     [ContextMenu("AddDeath")]
     public void AddDeath()
     {
         DeathsThisLevel++;
         HudDeaths.Instance.SetDeaths(DeathsThisLevel);
     }
     
     public void UpdateTotalProgress()
     {
         if (EnemyDirector.Instance.CurrentWave == null || EnemyDirector.Instance.CurrentWave.Enemies.Length == 0)
         {
             return;
         }
         
         float levelSegmentSize = 1 / (float)Levels.Infos.Length;
         float waveSegmentSize = (1 / (float)EnemyDirector.Instance.CurrentWaves.Length) * levelSegmentSize;
         //float enemiesSegmentSize = 1 / (float)EnemyDirector.Instance.CurrentWave.Enemies.Length;


         
         float levelProgress = LevelProgress / (float)Levels.Infos.Length;
         float waveProgress = (EnemyDirector.Instance.CurrentWaveIndex / (float)EnemyDirector.Instance.CurrentWaves.Length) * levelSegmentSize;
         float enemyProgress = (EnemyDirector.Instance.CurrentWaveEnemiesKilled / (float)EnemyDirector.Instance.CurrentWave.Enemies.Length) * waveSegmentSize;

         float progress = levelProgress + waveProgress + enemyProgress;

         //Debug.Log($"Level Segment Size: {levelSegmentSize:F3}");
         Debug.Log($"Level Progress: {levelProgress:F3}");
         Debug.Log($"Wave Progress: {waveProgress:F3}");
         Debug.Log($"Enemy Progress: {enemyProgress:F3} / ");
         //Debug.Log($"Total Progress: {(levelProgress + waveProgress + enemyProgress):F3}");

         ParallaxMaster.Instance.SetProgress(progress);
         MusicPlayer.Instance.SetProgress(progress);
         HudBarProgress.Instance.Set(progress);
         
     }

     private void OnGUI()
     {
         //return;
         /*if (!Debug.isDebugBuild)
         {
             return;
         }*/
        // Define dimensions and positions
        float barWidth = 200f;
        float barHeight = 20f;
        float padding = 10f;
        float startX = padding;
        float startY = padding;

        // Calculate progress values
        float levelProgress = LevelProgress / (float)Levels.Infos.Length;
        float waveProgress = (EnemyDirector.Instance.CurrentWaveIndex / (float)EnemyDirector.Instance.CurrentWaves.Length);
        float enemyProgress = (EnemyDirector.Instance.CurrentWaveEnemiesKilled / (float)EnemyDirector.Instance.CurrentWave.Enemies.Length);

        // Level progress bar
        GUI.Box(new Rect(startX - 2, startY - 2, barWidth + 4, barHeight + 4), ""); // Border
        GUI.backgroundColor = new Color(0.2f, 0.6f, 1f); // Blue
        GUI.Box(new Rect(startX, startY, barWidth * levelProgress, barHeight), "");
        GUI.Label(new Rect(startX, startY, barWidth, barHeight), $"Level Progress: {(levelProgress * 100):F0}%");

        // Wave progress bar
        startY += barHeight + padding;
        GUI.Box(new Rect(startX - 2, startY - 2, barWidth + 4, barHeight + 4), ""); // Border
        GUI.backgroundColor = new Color(0.2f, 1f, 0.2f); // Green
        GUI.Box(new Rect(startX, startY, barWidth * waveProgress, barHeight), "");
        GUI.Label(new Rect(startX, startY, barWidth, barHeight), $"Wave Progress: {(waveProgress * 100):F0}%");

        // Enemy progress bar
        startY += barHeight + padding;
        GUI.Box(new Rect(startX - 2, startY - 2, barWidth + 4, barHeight + 4), ""); // Border
        GUI.backgroundColor = new Color(1f, 0.6f, 0.2f); // Orange
        GUI.Box(new Rect(startX, startY, barWidth * enemyProgress, barHeight), "");
        GUI.Label(new Rect(startX, startY, barWidth, barHeight), $"Enemy Progress: {(enemyProgress * 100):F0}%");

        // Reset GUI background color
        GUI.backgroundColor = Color.white;
     }

     private void SetABackdrop(Background levelBackgroundToShow)
     {
         foreach (var bg in Backdrops)
         {
             bg.Image.gameObject.SetActive(false);
         }

         BackdropInstance backdrop = Backdrops.Find(p => p.Type == levelBackgroundToShow);
         if (backdrop != null)
         {
             backdrop.Image.gameObject.SetActive(true);
         }
     }

     public int DeathsThisLevel;
     public int CurrentTimeBonus;
     
     /// <summary>
     /// Indicate the score and what events changed the score. show it going down and up here.
     /// Ask the player to pick a weapon.
     /// </summary>
     /// <returns></returns>
     IEnumerator LevelTransition(string levelName, bool lastOne)
     {
         //Debug.Log("transition on the way to the ");
         InIntermission = true;
         
         HudManager.Instance.Toast($"{levelName} complete");
         yield return new WaitForSeconds(2);

         yield return TransitionUI.Instance.DoTallying(DeathsThisLevel);
         ResetTimeBonus();

         //skip picking an upgrade if we won
         if (!lastOne)
         {
             HandleIntermissionUpgrade();
             while (ChoosingNewUpgrade)
             {
                 yield return null;
             }
         }
         
         yield return new WaitForSeconds(1);
         InIntermission = false;
     }

     public Color GetElementColor(Element element)
     {
         return ElementColors[element];
     }
    
     public void Update()
     {
         if (!GameInProgress)
             return;

         if (InIntermission) return;
         
         TimeBonusDecrementTimer += Time.deltaTime;
         float interval = 1.5f;
         if (TimeBonusDecrementTimer > interval)
         {
             TimeBonusDecrementTimer -= interval;
             
             CurrentTimeBonus -= 10;
             HudManager.Instance.SetTimeBonusText(CurrentTimeBonus);
         }
     }

     public void ResetTimeBonus()
     {
         CurrentTimeBonus = 1000;
         TimeBonusDecrementTimer = 0;
         HudManager.Instance.SetTimeBonusText(CurrentTimeBonus);
         HudManager.Instance.TimeBonusText.gameObject.SetActive(true);
     }

     private float TimeBonusDecrementTimer;

     public void AddPoints(int points)
     {
         Score += points;
     }
     public void LosePoints(int points)
     {
         Score = Mathf.Max(Score - points, 0);
     }
}