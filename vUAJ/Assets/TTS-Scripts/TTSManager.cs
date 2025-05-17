using System.Transactions;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;

//Add this script to an empy object in the scene
public class TTSManager : MonoBehaviour
{
    //Ativar/desactivar TTS global
    [SerializeField]
    bool TTSactive = false;

    public static TTSManager m_Instance;

    [SerializeField]
    public KeyCode key_say = KeyCode.S; //Decir algo nuevo

    [SerializeField]
    public KeyCode key_repeat = KeyCode.R; //Repetir lo ya dicho

    [SerializeField]
    public string text;

    private void OnEnable()
    {
        if (m_Instance == null) { m_Instance = this; }
        else Destroy(this);
    }


    public void StartSpeech(string _text)
      => ttsrust_say(_text);

    //Importar DLL
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);


    //Leer subtitulos
    //Si opcion leer subtitulos activado = llamar a leer subtitulos

    //Leer notificacion
    //Si boton notificacion TTS activo = llamar a leer notificacion
    bool notifsActive;

    //Audiodescripcion
    //Si boton de audiodescripcion de escena activado = leer audiodescripcion
    //(del dev debe añadir el texto que quiere que se describa en cada momento)

    public void TTSActivation()
    {
        TTSactive = !TTSactive;
    }

    void Update()
    {
        //if (Input.GetKeyDown(key_say) && elapsedTime >= keyCooldown)
        //{
        //    StartSpeech(text);
        //    elapsedTime = 0f;
        //}
        //else
        //{
        //    elapsedTime += Time.deltaTime;
        //}
    }
}
