using TMPro;
using UnityEngine;

// Componente que tienen distintos elementos del HUD del menu de accesibilidad para
// llamar a distintas funciones de los managers
public class SettingFunctions : MonoBehaviour
{
    // Activa o desactiva el modo TTS (Text To Speech)
    public void setTTSMode()
    {
        TTSManager.Instance.TTSActivation();
    }

    // Activa o desactiva el modo Volume Perception (colliders resaltados con una malla de color)
    public void setShowColliders(System.Boolean show)
    {
        VolumePerceptionManager.Instance.setShowColliders(show);
    }

    // Activa o desactiva el modo dislexia friendly (todas los textos del juego cambian a la fuente elegida)
    public void setDislexiaMode(System.Boolean dislexiaMode)
    {
        TextAccesibilityManager.Instance.setDislexiaMode(dislexiaMode);
    }

    // Cambia el tamanyo de los textos
    public void onFontSizeChanged(System.Single value)
    {
        TextAccesibilityManager.Instance.onFontSizeChanged(value);
    }

    // Cambia la fuente de los textos
    public void onFontChanged(TMP_FontAsset font)
    {
        TextAccesibilityManager.Instance.onFontChanged(font);
    }
}
