using CamLib;
using UnityEngine.UI;

public class HudBarDifficulty : Singleton<HudBarDifficulty>
{
    public Image bar;
    
    public void UpdateBar(float difficulty)
    {
        bar.fillAmount = difficulty;
    }
}