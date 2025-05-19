using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public Transform canvasTransform;
    public UnityEngine.UI.Image blackOverlay;
    public UnityEngine.UI.Image transparencyOverlay;
    public GameObject currentHUD;

    void Start()
    {
        // Asegúrate de que el NotificationManager esté cargado y asigna el canvas
        if (NotificationManager.Instance != null && canvasTransform != null)
        {
            NotificationManager.Instance.SetCanvasPreferences(canvasTransform, blackOverlay, transparencyOverlay);
        }
        if(HUDManager.Instance != null && currentHUD != null)
        {
            HUDManager.Instance.loadCurrentHUD(currentHUD);
        }
    }
}
