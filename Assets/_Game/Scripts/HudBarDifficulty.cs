using CamLib;
using UnityEngine.UI;

public class HudBarDifficulty : Singleton<HudBarDifficulty>
{
    public Image bar;

    private void Start()
    {
        UpdateBar(0);
    }

    public void UpdateBar(float difficulty)
    {
        bar.fillAmount = difficulty;
    }
}