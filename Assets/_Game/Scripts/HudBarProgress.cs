using CamLib;
using UnityEngine;
using UnityEngine.UI;

public class HudBarProgress : Singleton<HudBarProgress>
{
    public Image Progress;

    private void Start()
    {
        Set(0);
    }

    public void Set(float progress)
    {
        Progress.fillAmount = progress;
    }
}