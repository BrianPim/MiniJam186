using System.Collections;
using CamLib;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Take the score, and chunk in some things.
/// </summary>
public class TransitionUI : Singleton<TransitionUI>
{
    public float FormerScoreY;
        
    public RectTransform ScoreText;
        
    public GameObject DeathsRoot;
    public RectTransform DeathsText;

    public RectTransform TimeBonus;

    public GameObject CrackedHeartPrefab;
    

    public IEnumerator DoTallying(int deathCount)
    {
        // Store initial positions
        FormerScoreY = ScoreText.localPosition.y;
        Vector3 initialTimePosition = TimeBonus.localPosition;
        Vector3 initialDeathPosition = DeathsText.localPosition;

        // Create sequence
        Sequence sequence = DOTween.Sequence();

        // Move score text down
        sequence.Append(ScoreText.DOLocalMoveY(FormerScoreY - 100f, 0.5f).SetEase(Ease.OutBack));

        // Show bonus elements
        sequence.AppendCallback(() => 
        {
            TimeBonus.gameObject.SetActive(true);
            DeathsRoot.SetActive(true);
        });

        // Move time bonus to score
        sequence.Append(TimeBonus.DOMove(ScoreText.position, 0.7f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // Pulse effect when merged
                ScoreText.DOScale(Vector3.one * 1.2f, 0.2f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        // Color pulse
                        var textComponent = ScoreText.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            DOTween.Sequence()
                                .Append(textComponent.DOColor(Color.green, 0.2f))
                                .Append(textComponent.DOColor(Color.white, 0.2f));
                        }
                    });
                TimeBonus.gameObject.SetActive(false);
            }));

        // Short pause
        sequence.AppendInterval(0.3f);

        // Move deaths text to score
        sequence.Append(DeathsText.DOMove(ScoreText.position, 0.7f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // Pulse effect when merged
                ScoreText.DOScale(Vector3.one * 1.2f, 0.2f)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() =>
                    {
                        // Color pulse
                        var textComponent = ScoreText.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            DOTween.Sequence()
                                .Append(textComponent.DOColor(Color.red, 0.2f))
                                .Append(textComponent.DOColor(Color.white, 0.2f));
                        }
                    });
                DeathsRoot.SetActive(false);
            }));

        // Wait for sequence to complete
        yield return sequence.WaitForCompletion();
    }

}
    
public static class TextMeshProExtensions
{
    /// <summary>
    /// Tweens the text color to the target color over the specified duration
    /// </summary>
    /// <param name="text">The TextMeshProUGUI component</param>
    /// <param name="endValue">Target color</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static Tweener DOColor(this TextMeshProUGUI text, Color endValue, float duration)
    {
        return DOTween.To(() => text.color,
            x => text.color = x,
            endValue,
            duration);
    }

    /// <summary>
    /// Pulses the text color between two colors
    /// </summary>
    /// <param name="text">The TextMeshProUGUI component</param>
    /// <param name="pulseColor">Color to pulse to</param>
    /// <param name="duration">Duration for each color change</param>
    /// <returns>The sequence containing the color pulse</returns>
    public static Sequence DOColorPulse(this TextMeshProUGUI text, Color pulseColor, float duration)
    {
        Color originalColor = text.color;
        return DOTween.Sequence()
            .Append(text.DOColor(pulseColor, duration))
            .Append(text.DOColor(originalColor, duration));
    }
}