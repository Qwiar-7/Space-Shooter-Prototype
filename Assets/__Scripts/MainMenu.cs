using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text scoreGT;
    public static int bestScore;
    private void Start()
    {
        ScoreManager.ShowBestScore();
    }

    public void LoadInfiniteLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
