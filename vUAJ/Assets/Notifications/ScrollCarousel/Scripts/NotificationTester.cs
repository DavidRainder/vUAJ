using UnityEngine;
using System.Collections;
using static NotificationUI;

public class NotificationTester : MonoBehaviour
{
    private NotificationManager manager;
    private float timer = 0f;
    private float interval = 10f;

    private void Start()
    {
        manager = NotificationManager.Instance;
        timer = interval; // Para que la primera salga al instante
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            LaunchTestNotification();
        }
    }

    private void LaunchTestNotification()
    {
        manager.SpawnNotification(
            "Prueba de notificación",
            "armor",                        // icono que esté en tu biblioteca
            new Color(0.2588f, 0.3882f, 0.8039f, 1f),                  // color
            null,                           // sin sonido
            NotificationPosition.TopCenter, // posición
            NotificationSize.Medium,        // tamaño
            NotificationDuration.Unlimited,  // duración
            HapticFeedbackType.Heavy,        // haptic feedback
            NotificationStyle.Black
        );
    }
}
