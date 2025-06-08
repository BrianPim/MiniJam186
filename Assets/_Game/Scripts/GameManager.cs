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

     public List<EnemyController> Enemies = new List<EnemyController>();
     
     public LevelInfos Levels;
     [Space]
     public PulseText PulseTextPrefab;
     
     private Dictionary<Element, Color> ElementColors = new Dictionary<Element, Color>();
     
     private bool GameInProgress;
     private bool Paused;
     private int Score;
    
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

     private IEnumerator GameProgression()
     {
         for (int i = 0; i < Levels.Infos.Length; i++)
         {
             LevelInfo level = Levels.Infos[i];
             Debug.Log($"startup level {level.LevelName}");
             LevelProgress = i;
             UpdateTotalProgress();

             HudManager.Instance.Toast(level.LevelName);

             SetABackdrop(level.BackgroundToShow);

             yield return EnemyDirector.Instance.SpawnWaves(level.Waves);

             yield return LevelTransition(level.LevelName);
         }
     }

     public float LevelProgress;
     
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
         return;
         if (!Debug.isDebugBuild)
         {
             return;
         }
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

     /// <summary>
     /// Indicate the score and what events changed the score. show it going down and up here.
     /// Ask the player to pick a weapon.
     /// </summary>
     /// <returns></returns>
     IEnumerator LevelTransition(string levelName)
     {
         //Debug.Log("transition on the way to the ");
         HudManager.Instance.Toast($"{levelName} complete");
         yield return new WaitForSeconds(2);
     }

     public Color GetElementColor(Element element)
     {
         return ElementColors[element];
     }
    
     public void Update()
     {
         if (!GameInProgress || Paused)
             return;
     }

     public void AddPoints(int points)
     {
         Score += points;
     }
}