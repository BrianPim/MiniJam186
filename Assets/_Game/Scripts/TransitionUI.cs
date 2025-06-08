using System.Collections;
using CamLib;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Take the score, and chunk in some things.
/// </summary>
public class TransitionUI : Singleton<TransitionUI>
{
    public float FormerScoreY;
        
    public RectTransform ScoreText;
    public Image ScoreBG;
        
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
        
        //dropdown score
        ScoreText.DOScale(2f, 1f).SetEase(Ease.OutBack);
        yield return ScoreText.DOLocalMoveY(FormerScoreY - 500f, 1f).SetEase(Ease.OutBack).WaitForCompletion();

        //time bonus
        yield return TimeBonus.DOMove(ScoreText.position, 1f).SetEase(Ease.InBack).WaitForCompletion();;
        PulseScoreText(Color.green);
        TimeBonus.gameObject.SetActive(false);
        TimeBonus.localPosition = initialTimePosition;
        
        yield return new WaitForSeconds(0.2f);
        
        if (deathCount > 0)
        {
            yield return SpawnHearts(deathCount);
        }
        
        //bring back score text
        ScoreText.DOScale(1f, 1f).SetEase(Ease.OutBack);
        yield return ScoreText.DOLocalMoveY(FormerScoreY, 1f).SetEase(Ease.OutBack).WaitForCompletion();;
        
    }

    public Tween CurrentColorPulse;
    
    private void PulseScoreText(Color pulseColor)
    {
        ScoreText.DOComplete();
        ScoreText.DOShakeScale(0.3f).SetEase(Ease.OutBounce);

        
        CurrentColorPulse?.Complete();
        
        ScoreBG.color = pulseColor;
        CurrentColorPulse = ScoreBG.DOColor(Color.white, 0.5f);
    }

    private IEnumerator SpawnHearts(int count)
    {
        int deathsLeft = count;

        for (int i = 0; i < count; i++)
        {
            deathsLeft--;
            HudDeaths.Instance.SetDeathsKeepUI(deathsLeft);
            
            if (i == count - 1)
            {
                yield return StartCoroutine(HeartMovement());
            }
            else
            {
                StartCoroutine(HeartMovement());
            }
            
            yield return new WaitForSeconds(0.3f);
        }
        
        IEnumerator HeartMovement()
        {
            GameObject heart = Instantiate(CrackedHeartPrefab, DeathsRoot.transform);
            RectTransform heartRect = heart.GetComponent<RectTransform>();
            
            
        
            // Random starting position around the deaths text
            //float randomX = Random.Range(-50f, 50f);
            //float randomY = Random.Range(-50f, 50f);
            heartRect.position = DeathsText.position;
            heart.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

            // Add some floating motion
            Vector2 midPoint = Vector2.Lerp(heartRect.position, ScoreText.position, 0.5f);
            midPoint += new Vector2(Random.Range(-40f, 40f), Random.Range(20f, 60f));
            yield return heartRect.DOMove(midPoint, 0.4f).SetEase(Ease.OutQuad).WaitForCompletion();
            
            
            heart.transform.DORotate(new Vector3(0, 0, Random.Range(-180f, 180f)), 0.7f, RotateMode.FastBeyond360);
            yield return heartRect.DOMove(ScoreText.position, 0.3f).SetEase(Ease.InQuad).WaitForCompletion();;
        
        
            //heart.transform.DOScale(Vector3.zero, 0.2f).SetDelay(0.5f).WaitForCompletion();
        
            PulseScoreText(Color.red);
            
            heart.gameObject.SetActive(false);
            Destroy(heart, 1);
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

public static class RectTransformExtensions
{
    /// <summary>
    /// Tweens the anchoredPosition of a RectTransform to the target position
    /// </summary>
    /// <param name="target">The RectTransform to tween</param>
    /// <param name="endValue">Target position</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchorPos(this RectTransform target, Vector2 endValue, float duration)
    {
        return DOTween.To(() => target.anchoredPosition,
            x => target.anchoredPosition = x,
            endValue,
            duration);
    }

    /// <summary>
    /// Tweens the anchoredPosition.x of a RectTransform to the target value
    /// </summary>
    /// <param name="target">The RectTransform to tween</param>
    /// <param name="endValue">Target x position</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static Tweener DOAnchorPosX(this RectTransform target, float endValue, float duration)
    {
        return DOTween.To(() => target.anchoredPosition.x,
            x => target.anchoredPosition = new Vector2(x, target.anchoredPosition.y),
            endValue,
            duration);
    }

    /// <summary>
    /// Tweens the anchoredPosition.y of a RectTransform to the target value
    /// </summary>
    /// <param name="target">The RectTransform to tween</param>
    /// <param name="endValue">Target y position</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static Tweener DOAnchorPosY(this RectTransform target, float endValue, float duration)
    {
        return DOTween.To(() => target.anchoredPosition.y,
            y => target.anchoredPosition = new Vector2(target.anchoredPosition.x, y),
            endValue,
            duration);
    }
}

public static class ImageExtensions
{
    /// <summary>
    /// Tweens the Image's color alpha to the target value
    /// </summary>
    /// <param name="image">The Image component to fade</param>
    /// <param name="endValue">Target alpha value (0-1)</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static Tweener DOFade(this Image image, float endValue, float duration)
    {
        return DOTween.To(() => image.color.a,
            alpha => image.color = new Color(image.color.r, image.color.g, image.color.b, alpha),
            endValue,
            duration);
    }

    /// <summary>
    /// Tweens the Image's color to the target color
    /// </summary>
    /// <param name="image">The Image component to tween</param>
    /// <param name="endValue">Target color</param>
    /// <param name="duration">Duration of the tween in seconds</param>
    /// <returns>The tween for further chaining</returns>
    public static Tweener DOColor(this Image image, Color endValue, float duration)
    {
        return DOTween.To(() => image.color,
            color => image.color = color,
            endValue,
            duration);
    }
}

