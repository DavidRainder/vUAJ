using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public Transform canvasTransform;
    public UnityEngine.UI.Image blackOverlay;
    public UnityEngine.UI.Image transparencyOverlay;

    void Start()
    {
        // Asegúrate de que el NotificationManager esté cargado y asigna el canvas
        if (NotificationManager.Instance != null && canvasTransform != null)
        {
            NotificationManager.Instance.SetCanvasPreferences(canvasTransform, blackOverlay, transparencyOverlay);
        }
    }
}
