using UnityEngine;

/// <summary>
/// Esta clase se encarga de inicializar y actualizar las referencias necesarias en los managers del juego
/// una vez que la escena actual ha sido cargada. En particular, asigna los elementos del Canvas para el 
/// sistema de notificaciones y carga el HUD actual. Estas referencias no pueden establecerse antes, ya que
/// los objetos (como el Canvas o HUD) solo existen dentro de esta escena específica.
/// </summary>
public class GameSceneController : MonoBehaviour
{
    public Transform canvasTransform; // Posición en el canvas del juego que se usa para instanciar la notificacion
    public UnityEngine.UI.Image blackOverlay; // Capa negra usada para oscurecer la pantalla 
    public UnityEngine.UI.Image transparencyOverlay; // Capa de transparencia 
    public GameObject currentHUD; // Prefab o instancia del HUD actual que se debe cargar

    void Start()
    {
        // Configura el NotificationManager con el canvas y overlays de la escena actual
        if (NotificationManager.Instance != null && canvasTransform != null)
        {
            NotificationManager.Instance.SetCanvasPreferences(canvasTransform, blackOverlay, transparencyOverlay);
        }
        // Carga el HUD actual a través del HUDManager
        if (HUDManager.Instance != null && currentHUD != null)
        {
            HUDManager.Instance.loadCurrentHUD(currentHUD);
        }
    }
}
