using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public GameObject Autor;
    private void Update()
    {
        if (Autor.activeInHierarchy && Input.GetAxis("Jump") == 1 || Input.touchCount > 0)
            InMainMenu();
    }
    public void InMainMenu()
    {
        WinScenario.isActive = false;
        SceneManager.LoadScene("MainMenuScene");
    }
}