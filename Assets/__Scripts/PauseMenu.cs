using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject pauseMenuUI;

    [Header("Set Dynamically")]
    public bool isGamePaused = false;
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
                Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        isGamePaused = false;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        Cursor.lockState= CursorLockMode.None;
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
