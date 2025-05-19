using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public void goToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void goToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
