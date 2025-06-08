using CamLib;
using UnityEngine;
using UnityEngine.UI;

public class HudBarEvolution : Singleton<HudBarEvolution>
{
    public Image[] bars;
    
    private void Start()
    {
        UpdateBars(new []{1f,1f,1f,1f});
    }
    
    //normal, fire, ice, electric
    public void UpdateBars(float[] weights)
    {
        float cumulativeWeight = 0f;

        for (int i = 0; i < bars.Length; i++)
        {
            cumulativeWeight += weights[i];
            bars[i].fillAmount = cumulativeWeight;
        }
    }
}