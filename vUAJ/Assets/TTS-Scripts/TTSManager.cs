using System.Transactions;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;

//Para que la opcion de TTS exista, hay que añadir este script a un objeto vacio en escena
//Se encarga de importar la DLL y leer lo que se le diga
public class TTSManager : MonoBehaviour
{
    //Ativar/desactivar TTS global
    [SerializeField]
    bool TTSactive = false;

    //Instancia de TTS Manager
    public static TTSManager m_Instance;

    private void OnEnable()
    {
        if (m_Instance == null) { m_Instance = this; }
        else Destroy(this);
    }

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
    public void TTSActivation()
    {
        TTSactive = !TTSactive;
    }
}
