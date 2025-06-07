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

    public Canvas GameHudCanvas;
    public Image ColourOverlay;

    public TextMeshProUGUI ScoreText;
    
    public TMP_Text ToastText;

    protected override void Awake()
    {
        base.Awake();

        ScoreText.enabled = false;
        
        ToastText.transform.localScale = Vector3.zero;
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
        ToastText.text = msg;
        ToastText.transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
    
        sequence.Append(ToastText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack))
            .AppendInterval(2f)
            .Append(ToastText.transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack));
    }
}