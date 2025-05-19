using UnityEngine;

//Esta clase se encarga de hacer TTS de notificaciones. Con la opcion de repetirla pulsando una tecla
//Se debe añadir a un objeto en escena para que funcione
public class TTSNotifications : MonoBehaviour
{


    [SerializeField]
    public float keyCooldown = 2f;
    public float elapsedTime = 0f;

    [SerializeField]
    public KeyCode key_repeat = KeyCode.R; //Repetir lo ya dicho

    string lastNotif = "";

    //Dice la notificacion y guarda la ultima notificacion dicha por si se quiere repetir
    public void SayNotification(string text)
    {
        if (!TTSManager.Instance.GetTTSactive()) return;

        TTSManager.Instance.StartSpeech(text);
        lastNotif = text;
    }

    //Se encarga de gestionar si se quiere repetir o no la notificacion
    private void Update()
    {
        if (Input.GetKeyDown(key_repeat) && elapsedTime >= keyCooldown) {
            SayNotification(lastNotif);
            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
