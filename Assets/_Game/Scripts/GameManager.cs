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
     
     private bool GameInProgress;
     private bool Paused;
     private int Score;
    
     public int GetScore => Score;
     
     
     
     protected override void Awake()
     {
         base.Awake();

         Player.gameObject.SetActive(false);
     }
    
     public void StartGame() 
     { 
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
         foreach (LevelInfo level in Levels.Infos)
         {
             Debug.Log($"startup level {level.LevelName}");
             
             HudManager.Instance.Toast(level.LevelName);

             SetABackdrop(level.BackgroundToShow);

             yield return EnemyDirector.Instance.SpawnWaves(level.Waves);

             yield return LevelTransition(level.LevelName);
         }
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
         yield return null;
     }
    
    
     public void Update()
     {
         if (!GameInProgress || Paused)
             return;
     }
}