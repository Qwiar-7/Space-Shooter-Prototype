using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject BestTime;
    public GameObject BestScore;
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject firstSelectedButton;
    public GameObject firstOptionsButton;
    public GameObject closingOptionsButton;

    [Header("Set Dynamically")]
    public Text scoreGT;
    public Text bestTimeText;
    public static float bestTime;
    public bool isEventUsing = false;
    private void Awake()
    {
        bestTimeText = BestTime.GetComponent<Text>();
        BestTime.SetActive(false);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ScoreManager.ShowBestScore();
        if (PlayerPrefs.HasKey("BestTime"))
        {
            bestTime = PlayerPrefs.GetFloat("BestTime");
            var span = TimeSpan.FromSeconds(bestTime);
            bestTimeText.text = "Best time:\n" + string.Format("{0:00}:{1:00}:{2:000}", (int)span.Minutes, span.Seconds, span.Milliseconds);
            BestTime.SetActive(true);
        }
        else
        {
            BestTime.SetActive(false);
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    public void LoadLevel_0()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOptionsButton);
    }
    public void CloseOptions()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(closingOptionsButton);
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