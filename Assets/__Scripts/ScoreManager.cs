using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Set Dynamically")]
    public static int bestScore = 0;
    public static int currentScore = 0;
    public static GameObject BestScore;
    public static GameObject CurrentScore;
    public static Text currentScoreText;
    public static Text bestScoreText;
    private void Awake()
    {
        BestScore = GameObject.Find("BestScore");
        CurrentScore = GameObject.Find("CurrentScore");
        if (BestScore != null)
        {
            bestScoreText = BestScore.GetComponent<Text>();
            if (PlayerPrefs.HasKey("BestScore"))
                bestScore = PlayerPrefs.GetInt("BestScore");
            BestScore.SetActive(false);
        }
        if (CurrentScore != null)
        {
            currentScoreText = CurrentScore.GetComponent<Text>();
            currentScoreText.text = "0";
        }
        currentScore = 0;
    }
    public static void ShowBestScore()
    {
        if (bestScoreText == null) return;
        if (bestScore < currentScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
        }
        bestScoreText.text = "Best Score: " + bestScore;
        BestScore.SetActive(true);
    }
    public static void ShowCurrentScore()
    {
        currentScoreText.text = "Current Score: " + currentScore;
    }
    public static void UpdateCurrentScore(int score)
    {
        currentScore += score;
        currentScoreText.text = currentScore.ToString();
    }
}