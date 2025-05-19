using UnityEngine;
using UnityEngine.SceneManagement;

// Componente que tienen los botones de las distintas escenas para cambiar entre
// menus y juego
public class SceneButton : MonoBehaviour
{
    // Activa la escena de Menu de Accesibilidad
    public void goToSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    // Activa la escena de Juego
    public void goToGame()
    {
        SceneManager.LoadScene("Game");
    }

    // Activa la escena de Main Menu
    public void goToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
