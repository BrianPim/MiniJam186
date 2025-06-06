using CamLib;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //Constants
    //------------------------------------------------------------

    public const float MaxLogoScale = 1.1f;
    public const float MinLogoScale = 1f;
    public const float LogoScaleDuration = 5f;

    //------------------------------------------------------------

    public Canvas Canvas;

    public Button PlayButton;
    public Button ExitButton;
    public Transform Logo;
        
    [NonSerialized] public bool ScalingUp = true;
    [NonSerialized] public float Elapsed;

    public void Update()
    {
        Elapsed += Time.deltaTime;

        if (Elapsed < LogoScaleDuration)
        {
            var scale = ScalingUp ? Mathf.Lerp(MinLogoScale, MaxLogoScale, Elapsed / LogoScaleDuration) : Mathf.Lerp(MaxLogoScale, MinLogoScale, Elapsed / LogoScaleDuration);

            Logo.localScale = Vector3.one * scale;
        }
        else
        {
            ScalingUp = !ScalingUp;
            Elapsed = 0;
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play");
        GameManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}