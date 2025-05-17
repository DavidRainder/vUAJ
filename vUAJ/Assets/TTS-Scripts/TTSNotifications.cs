using UnityEngine;

public class TTSNotifications : MonoBehaviour
{
    [SerializeField]
    public float keyCooldown = 2f;
    public float elapsedTime = 2f;

    [SerializeField]
    public KeyCode key_repeat = KeyCode.R; //Repetir lo ya dicho

    public string notifText;


    // Update is called once per frame
    void Update()
    {
        //if active y notificacion aparecce, dice la notificacion
            //if r pulsada, repetir texto notificacion
    }
}
