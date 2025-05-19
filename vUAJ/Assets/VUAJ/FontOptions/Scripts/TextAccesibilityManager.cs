using System;
using TMPro;
using UnityEngine;

// Manager que controla todas las opciones referentes a la accesibilidad de textos
// incluyendo: tamanyo y fuente
public class TextAccesibilityManager : MonoBehaviour
{
    // Codigo de Singleton
    #region Singleton
    private static TextAccesibilityManager _instance = null;

    public static TextAccesibilityManager Instance
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

    // Actions que se lanzan al cambiar la fuente o el tamanyo en el menu de accesibilidad
    public event Action<TMP_FontAsset> fontChange;
    public event Action<float> fontSizeChange;

    // Referencias a la fuente y tamanyo seleccionados 
    public TMP_FontAsset currentFontAsset;
    public float currentFontSize;

    // Booleano que indica si se aplican las fuentes aptas para dislexia o no
    public bool applyFonts = false;

    // Activa o desactiva el modo dislexia friendly (todas los textos del juego cambian a la fuente elegida)
    public void setDislexiaMode(bool dislexiaMode)
    {
        applyFonts = dislexiaMode;
        if (!applyFonts) fontChange(null);
        else fontChange(currentFontAsset);
    }

    // Cambia el tamanyo de los textos
    public void onFontSizeChanged(float value)
    {
        currentFontSize = value;
        fontSizeChange(value);
    }

    // Cambia la fuente de los textos
    public void onFontChanged(TMP_FontAsset font)
    {
        currentFontAsset = font;
        if (applyFonts) fontChange(font);
    }
}
