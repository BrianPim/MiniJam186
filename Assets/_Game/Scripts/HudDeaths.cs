using CamLib;
using TMPro;
using UnityEngine;

public class HudDeaths : Singleton<HudDeaths>
{
    public TMP_Text text;
    public CanvasGroup canvasGroup;

    private void Start()
    {
        SetDeaths(0);
    }

    public void SetDeaths(int deaths)
    {
        canvasGroup.alpha = deaths > 0 ? 1f : 0f;

        text.text = deaths.ToString();
            
        
    }

    public void SetDeathsKeepUI(int deaths)
    {
        text.text = deaths.ToString();
    }
}