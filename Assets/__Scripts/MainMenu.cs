using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject BestTime;
    public GameObject BestScore;
    public GameObject mainMenu;
    public GameObject optionsMenu;

    [Header("Set Dynamically")]
    public Text scoreGT;
    public Text bestTimeText;
    public static float bestTime;
    private void Awake()
    {
        bestTimeText = BestTime.GetComponent<Text>();
        BestTime.SetActive(false);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        ScoreManager.ShowBestScore();
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
            var span = TimeSpan.FromSeconds(bestTime);
            bestTimeText.text = "Лучшее время:\n" + string.Format("{0:00}:{1:00}:{2:000}", (int)span.Minutes, span.Seconds, span.Milliseconds);
            BestTime.SetActive(true);
        }
        else
        {
            BestTime.SetActive(false);
        }
    }
    public void LoadLevel_0()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void CloseOptions()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        mainMenu.transform.GetChild(2).GetComponent<Animator>().Play("Resume_Button", -1, 0f);
    }
    public void ClearPlayerStatistics()
    {
        PlayerPrefs.DeleteKey("BestTime");
        PlayerPrefs.DeleteKey("BestScore");
        ScoreManager.bestScore = 0;
        ScoreManager.ShowBestScore();
        BestTime.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}