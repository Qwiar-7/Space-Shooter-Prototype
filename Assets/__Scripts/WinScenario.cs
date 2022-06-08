using UnityEngine;
using UnityEngine.UI;
using System;
public class WinScenario : MonoBehaviour
{
    public static WinScenario main;
    [Header("Set in Inspector")]
    public GameObject creditsPanel;
    public GameObject autor;
    public GameObject timeToComplete;

    [Header("Set Dynamically")]
    public float currentLeveltime;
    public static bool isActive = false;
    public static float bestTime = float.MaxValue;
    private void Awake()
    {
        main = this;
        if (PlayerPrefs.HasKey("BestTime"))
            bestTime = PlayerPrefs.GetFloat("BestTime");
    }
    public void EndLevel()
    {
        ScoreManager.ShowBestScore();
        ScoreManager.ShowCurrentScore();
        CancelInvoke();
        Invoke(nameof(Credits), 4f);
        Invoke(nameof(AutorAndTime), 5f);
    }
    public void Credits()
    {
        creditsPanel.SetActive(true);
        isActive = true;
    }
    public void AutorAndTime()
    {
        currentLeveltime = Time.time - Main.startTime - 5f;
        if (bestTime > currentLeveltime)
        {
            bestTime = currentLeveltime;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }
        var span = TimeSpan.FromSeconds(currentLeveltime);
        timeToComplete.GetComponent<Text>().text = "Время прохождения:\n" + string.Format("{0:00}:{1:00}:{2:000}", (int)span.Minutes, span.Seconds, span.Milliseconds);
        autor.SetActive(true);
        timeToComplete.SetActive(true);
    }
}