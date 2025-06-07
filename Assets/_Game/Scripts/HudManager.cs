using System;
using CamLib;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HudManager : Singleton<HudManager>
{
    public MainMenu MainMenu;
    [Space]
    public Canvas GameHudCanvas;
    public Image ColourOverlay;
    [Space]
    public Transform WeaponWheelRotator;
    public Image[] WeaponWheelImages;
    public Image WeaponWheelSelected;
    [Space] 
    public GameObject[] HideUntilGameStarts;

    public TextMeshProUGUI ScoreText;
    
    public TMP_Text ToastText;

    private Coroutine ActiveWeaponWheelRoutine;

    protected override void Awake()
    {
        base.Awake();

        ScoreText.enabled = false;
        
        ToastText.transform.localScale = Vector3.zero;

        HideGameHud(true);
    }

    public void Update()
    {
        ScoreText.text = $"Score: {GameManager.Instance.GetScore}";
    }

    public void ActivateGameHud()
    {
        ScoreText.enabled = true;
    }

    public void FadeIntoBlack(float duration)
    {
        IEnumerator FadeRoutine()
        {
            ColourOverlay.color = new Color(0, 0, 0, 0);

            for (int i = 0; i < 100; i++)
            {
                var alpha = Mathf.Lerp(0f, 1f, i / 100f);
                ColourOverlay.color = new Color(0, 0, 0, alpha);

                yield return new WaitForSeconds(.01f * duration);
            }
        }

        StartCoroutine(FadeRoutine());
    }

    public void FadeOutOfBlack(float duration)
    {
        IEnumerator FadeRoutine()
        {
            ColourOverlay.color = new Color(0, 0, 0, 1);

            for (int i = 0; i < 100; i++)
            {
                var alpha = Mathf.Lerp(1f, 0f, i / 100f);
                ColourOverlay.color =  new Color(0, 0, 0, alpha);

                yield return new WaitForSeconds(.01f * duration);
            }
        }

        StartCoroutine(FadeRoutine());
    }
    
    public void Toast(string msg)
    {
        if (msg.IsNullOrEmpty()) return;
        
        ToastText.text = msg;
        ToastText.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
    
        sequence.Append(ToastText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack))
            .AppendInterval(1f)
            .Append(ToastText.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack));
    }

    public void HideGameHud(bool hide)
    {
        foreach (var obj in HideUntilGameStarts)
        {
            obj.SetActive(!hide);
        }
    }

    public void UpdateWeaponWheel(int oldIndex)
    {
        var newIndex = GameManager.Instance.Player.GetCurrentWeaponIndex();
        
        IEnumerator WeaponWheelRoutine()
        {
            //var speedModifier = Math.Abs(newIndex - oldIndex);

            WeaponWheelSelected.enabled = false;
            var targetRotation = 0f;

            switch (newIndex)
            {
                case 1:
                    targetRotation = -90f;
                    break;
                case 2:
                    targetRotation = -180f;
                    break;
                case 3:
                    targetRotation = -270f;
                    break;
                default:
                    targetRotation = -360f;
                    break;
            }
            
            while (WeaponWheelRotator.rotation != Quaternion.Euler(0, 0, targetRotation))
            {
                WeaponWheelRotator.Rotate(Vector3.forward, -1);
                
                yield return new WaitForSeconds(.002f);
            }

            if (newIndex == 0)
            {
                WeaponWheelRotator.rotation = Quaternion.identity;    
            }
            
            WeaponWheelSelected.enabled = true;
        }
        
        if (newIndex == 0 && oldIndex == 0)
        {
            return;
        }

        if (ActiveWeaponWheelRoutine != null)
        {
            StopCoroutine(ActiveWeaponWheelRoutine);    
        }
        
        ActiveWeaponWheelRoutine = StartCoroutine(WeaponWheelRoutine());
    }
}