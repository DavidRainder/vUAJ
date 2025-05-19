using UnityEngine;

/// <summary>
/// GameManager es un singleton que sirve como base para la gestión global del juego. 
/// 
/// **Uso del GameManager como base de la lógica del juego:**
/// El GameManager proporciona una estructura básica que los desarrolladores pueden ampliar con la lógica interna de su juego. 
/// Aunque puede ser utilizado tal cual, se recomienda extenderlo para incluir las características específicas del juego que se está desarrollando.
/// </summary>
public class vUAJBaseGameManager : MonoBehaviour
{
    // Singleton instance
    #region Singleton
    private static vUAJBaseGameManager _instance = null;

    public static vUAJBaseGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Accesibility Manager not present in scene");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// La propiedad `GameIsPaused` es un valor booleano que indica si el juego está actualmente en pausa. 
    /// Todos los objetos de la escena deben verificar si `GameManager.Instance.GameIsPaused` es verdadero antes de ejecutar sus 
    /// lógicas de actualización o interacción. Esto garantiza que las notificaciones y otros eventos importantes no interfieran 
    /// con la jugabilidad cuando el juego está pausado. Se recomienda utilizar este patrón para gestionar el estado global del juego.
    /// </summary>
    public bool GameIsPaused = false; 
}
