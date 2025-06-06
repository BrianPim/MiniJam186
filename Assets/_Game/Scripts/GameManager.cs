using System;
using System.Collections;
using CamLib;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;
    
     public EdgeCollider2D MovementBoundaries;
     public Transform Background;
    
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
             
             HudManager.Instance.FadeOutOfBlack(1);
    
             yield return new WaitForSeconds(1);
    
             GameInProgress = true;
         }
    
         StartCoroutine(GameTransition());
     }
    
     public void Update()
     {
         if (!GameInProgress || Paused)
             return;
     }
}