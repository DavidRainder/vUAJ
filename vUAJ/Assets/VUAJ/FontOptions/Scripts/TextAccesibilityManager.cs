using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextAccesibilityManager : MonoBehaviour
{
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

    public event Action<TMP_FontAsset> fontChange;
    public event Action<float> fontSizeChange;

    public TMP_FontAsset currentFontAsset;
    public float currentFontSize;
    public bool applyFonts = false;

    public void setDislexiaMode(bool dislexiaMode)
    {
        applyFonts = dislexiaMode;
        if (!applyFonts) fontChange(null);
        else fontChange(currentFontAsset);
    }

    public void onFontSizeChanged(float value)
    {
        currentFontSize = value;
        fontSizeChange(value);
    }

    public void onFontChanged(TMP_FontAsset font)
    {
        currentFontAsset = font;
        if (applyFonts) fontChange(font);
    }
}
