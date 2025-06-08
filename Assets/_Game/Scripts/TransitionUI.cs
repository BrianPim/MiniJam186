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
                PulseScoreText(Color.green);
                TimeBonus.gameObject.SetActive(false);
            }));

        // Short pause
        sequence.AppendInterval(0.3f);

        // Spawn and animate hearts
        sequence.AppendCallback(() =>
        {
            StartCoroutine(SpawnHearts(deathCount));
        });

        // Move deaths text to score
        sequence.Append(DeathsText.DOMove(ScoreText.position, 0.7f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                PulseScoreText(Color.red);
                DeathsRoot.SetActive(false);
            }));

        // Wait for sequence to complete
        yield return sequence.WaitForCompletion();
    }

    private void PulseScoreText(Color pulseColor)
    {
        ScoreText.DOScale(Vector3.one * 1.2f, 0.2f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                var textComponent = ScoreText.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.DOColorPulse(pulseColor, 0.2f);
                }
            });
    }

    private IEnumerator SpawnHearts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject heart = Instantiate(CrackedHeartPrefab, DeathsRoot.transform);
            RectTransform heartRect = heart.GetComponent<RectTransform>();
        
            // Random starting position around the deaths text
            float randomX = Random.Range(-50f, 50f);
            float randomY = Random.Range(-50f, 50f);
            heartRect.anchoredPosition = DeathsText.anchoredPosition + new Vector2(randomX, randomY);
        
            // Random rotation
            heart.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));
        
            // Create heart movement sequence
            Sequence heartSequence = DOTween.Sequence();
        
            // Add some floating motion
            Vector2 midPoint = Vector2.Lerp(heartRect.anchoredPosition, ScoreText.anchoredPosition, 0.5f);
            midPoint += new Vector2(Random.Range(-40f, 40f), Random.Range(20f, 60f));
        
            // Path to score with arc motion
            heartSequence.Append(heartRect.DOAnchorPos(midPoint, 0.4f).SetEase(Ease.OutQuad))
                        .Append(heartRect.DOAnchorPos(ScoreText.anchoredPosition, 0.3f).SetEase(Ease.InQuad));
        
            // Rotate during movement
            heartSequence.Join(heart.transform.DORotate(
                new Vector3(0, 0, Random.Range(-180f, 180f)), 
                0.7f, 
                RotateMode.FastBeyond360)
            );
        
            // Scale down at the end
            heartSequence.Join(heart.transform.DOScale(Vector3.zero, 0.2f).SetDelay(0.5f));
        
            // Trigger score pulse when heart reaches it
            heartSequence.OnComplete(() =>
            {
                PulseScoreText(Color.red);
                Destroy(heart);
            });

            yield return new WaitForSeconds(0.1f); // Delay between heart spawns
        }
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