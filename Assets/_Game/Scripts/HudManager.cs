using System;
using CamLib;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class UpgradePanelData
{
    public Upgrade Upgrade;
    public Sprite Sprite;
    public string NameText;
    public string DescriptionText;
}

public class HudManager : Singleton<HudManager>
{
    public MainMenu MainMenu;
    [Space]
    public Image ColourOverlay;
    [Space]
    public Transform WeaponWheelRotator;
    public Image[] WeaponWheelImages;
    public Image[] WeaponWheelImagesSelected;
    [Space] 
    public GameObject[] HideUntilGameStarts;

    [Space] 
    public GameObject UpgradeScreen;
    public GameObject[] UpgradeScreenPanels;
    public Image[] UpgradeScreenImages;
    public TextMeshProUGUI[] UpgradeScreenNameTexts;
    public TextMeshProUGUI[] UpgradeScreenDescriptionTexts;

    public TextMeshProUGUI ScoreText;
    
    public TMP_Text ToastText;
    
    [Space]
    
    [SerializeField] private List<UpgradePanelData> UpgradePanelDataList;

    private Coroutine ActiveWeaponWheelRoutine;

    protected override void Awake()
    {
        base.Awake();

        ScoreText.enabled = false;
        
        ToastText.transform.localScale = Vector3.zero;

        HideGameHud(true);
        
        foreach (Image image in WeaponWheelImagesSelected)
        {
            image.enabled = false;
        }
        WeaponWheelImagesSelected[0].enabled = true;
        
        UpgradeScreen.SetActive(false);
    }

    public void Update()
    {
        ScoreText.text = $"{GameManager.Instance.GetScore}";
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
        Debug.Log($"toast \"{msg}\"");
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
            foreach (Image image in WeaponWheelImagesSelected)
            {
                image.enabled = false;
            }
            
            
            var targetRotation = 0f;
            
            GameManager.Instance.Player.SetAllowSwitchWeapons(false);

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
            
            WeaponWheelImagesSelected[newIndex].enabled = true;
            GameManager.Instance.Player.SetAllowSwitchWeapons(true);
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

    public void ShowUpgradeScreen(bool show, bool refresh = true)
    {
        if (show)
        {
            UpgradeScreen.SetActive(true);

            if (refresh)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (GameManager.Instance.IntermissionUpgradeOptions.Count <= i)
                    {
                        UpgradeScreenPanels[i].SetActive(false);
                    }
                    
                    UpgradePanelData data = UpgradePanelDataList.Find(p => p.Upgrade == GameManager.Instance.IntermissionUpgradeOptions[i]);

                    UpgradeScreenImages[i].sprite = data.Sprite;
                    UpgradeScreenNameTexts[i].text = data.NameText;
                    UpgradeScreenDescriptionTexts[i].text = data.DescriptionText;
                    
                    UpgradeScreenPanels[i].SetActive(true);
                }
            }
        }
        else
        {
            UpgradeScreen.SetActive(false);
        }
    }

    public void ChooseUpgrade1()
    {
        GameManager.Instance.HandleAddUpgrade(GameManager.Instance.IntermissionUpgradeOptions[0]);
        
        UpgradeScreen.SetActive(false);
    }

    public void ChooseUpgrade2()
    {
        GameManager.Instance.HandleAddUpgrade(GameManager.Instance.IntermissionUpgradeOptions[1]);
        
        UpgradeScreen.SetActive(false);
    }

    public void ChooseUpgrade3()
    {
        GameManager.Instance.HandleAddUpgrade(GameManager.Instance.IntermissionUpgradeOptions[2]);
        
        UpgradeScreen.SetActive(false);
    }

    private void LateUpdate()
    {
        foreach (Image image in WeaponWheelImages)
        {
            image.transform.rotation = Quaternion.identity;
        }
        foreach (Image image in WeaponWheelImagesSelected)
        {
            image.transform.rotation = Quaternion.identity;
        }
    }

    public TMP_Text TimeBonusText;
    public void SetTimeBonusText(int timeBonus)
    {
        TimeBonusText.text = timeBonus.ToString();
    }
}