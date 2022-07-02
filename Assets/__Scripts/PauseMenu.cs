using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject pauseMenuUI;
    public GameObject firstSelectedButton;

    [Header("Set Dynamically")]
    public bool isGamePaused = false;
    public bool isEventUsing = false;
    private void Update()
    {
        if (WinScenario.isActive)
        {
            Resume();
            return;
        }
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (isGamePaused)
                Resume();
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
    public void InMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
