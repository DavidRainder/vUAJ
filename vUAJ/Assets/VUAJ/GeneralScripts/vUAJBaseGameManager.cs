using UnityEngine;

/// <summary>
/// GameManager es un singleton que sirve como base para la gesti�n global del juego. 
/// 
/// **Uso del GameManager como base de la l�gica del juego:**
/// El GameManager proporciona una estructura b�sica que los desarrolladores pueden ampliar con la l�gica interna de su juego. 
/// Aunque puede ser utilizado tal cual, se recomienda extenderlo para incluir las caracter�sticas espec�ficas del juego que se est� desarrollando.
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
    /// La propiedad `GameIsPaused` es un valor booleano que indica si el juego est� actualmente en pausa. 
    /// Todos los objetos de la escena deben verificar si `GameManager.Instance.GameIsPaused` es verdadero antes de ejecutar sus 
    /// l�gicas de actualizaci�n o interacci�n. Esto garantiza que las notificaciones y otros eventos importantes no interfieran 
    /// con la jugabilidad cuando el juego est� pausado. Se recomienda utilizar este patr�n para gestionar el estado global del juego.
    /// </summary>
    public bool GameIsPaused = false; 
}
