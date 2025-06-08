using CamLib;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalRankingUI : Singleton<FinalRankingUI>
{
    public TMP_Text RankingText;
    public TMP_Text ScoreText;

    public Button PlayAgainButton;
    
    private void Start()
    {
        gameObject.SetActive(false);
        
        PlayAgainButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void Show(int points)
    {
        gameObject.SetActive(true);
        ScoreText.text = points.ToString();
        RankingText.text = FinalRankingManager.Instance.GetRanking(points);
    }
}