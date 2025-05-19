using System.Transactions;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;

//Para que la opcion de TTS exista, hay que a�adir este script a un objeto vacio en escena
//Se encarga de importar la DLL y leer lo que se le diga
public class TTSManager : MonoBehaviour
{
    //Ativar/desactivar TTS global
    [SerializeField]
    bool TTSactive = false;


    #region Singleton
    private static TTSManager _instance = null;

    public static TTSManager Instance
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

    //Funcion que se encarga de que se diga el texto que se le pase
    public void StartSpeech(string _text)
    {
        if (TTSactive)
        {
            ttsrust_say(_text);
        }
    }

    //Importar DLL
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_WEBGL)
    const string _dll = "__Internal";
#else
    const string _dll = "ttsrust";
#endif

    [DllImport(_dll)] static extern void ttsrust_say(string text);

    //Funcion que activa o desactiva las funciones de TTS desde un menu de ajustes general
    public bool GetTTSactive() { return TTSactive; }
    public void TTSActivation()
    {
        TTSactive = !TTSactive;
    }
}
