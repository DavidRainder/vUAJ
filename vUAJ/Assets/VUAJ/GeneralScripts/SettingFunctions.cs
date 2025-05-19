using UnityEngine;
using UnityEngine.UI;

// Componente que tienen distintos elementos del HUD del menu de accesibilidad para
// llamar a distintas funciones de los managers
public class SettingFunctions : MonoBehaviour
{
    // Referencia al script de guardado
    Serializer serializer;

    [System.Serializable]
    struct ToggleSerializedInfo
    {
        public bool isOn;
    }

    [System.Serializable]
    struct FontSizeSerializedInfo
    {
        public float fontSize;
    }

    string fontSizeInfoPath = "FontSize";

    private void Start()
    {
        serializer = GetComponent<Serializer>();
        Toggle toggle = GetComponent<Toggle>();
        if (toggle != null && 
            serializer.getFromJSONStruct("SettingsMenu/" + gameObject.name, out ToggleSerializedInfo tg) != -1)
        {
            toggle.isOn = tg.isOn;
        }

        if (serializer.getFromJSONStruct("SettingsMenu/" + fontSizeInfoPath, out FontSizeSerializedInfo fS) != -1)
        {
            onFontSizeChanged(fS.fontSize);
        }
    }

    void serializeToJSONToggle()
    {
        serializer.Clear();
        var toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            ToggleSerializedInfo toggleSerializedInfo = new ToggleSerializedInfo();
            toggleSerializedInfo.isOn = toggle.isOn;
            serializer.Serialize(toggleSerializedInfo);
            serializer.WriteToJSON("SettingsMenu", gameObject.name);
        }
        else Debug.LogWarning("El objeto no tiene asociado un componente Toggle");
    }

    void serializeToJSONFontSize(float size)
    {
        serializer.Clear();
        FontSizeSerializedInfo fontSizeInfo = new FontSizeSerializedInfo();
        fontSizeInfo.fontSize = size;
        serializer.Serialize(fontSizeInfo);
        serializer.WriteToJSON("SettingsMenu", fontSizeInfoPath);
    }

    // Activa o desactiva el modo TTS (Text To Speech)
    public void setTTSMode()
    {
        TTSManager.Instance.TTSActivation();

        serializeToJSONToggle();
    }

    // Activa o desactiva el modo Volume Perception (colliders resaltados con una malla de color)
    public void setShowColliders(System.Boolean show)
    {
        VolumePerceptionManager.Instance.setShowColliders(show);

        serializeToJSONToggle();
    }

    // Activa o desactiva el modo dislexia friendly (todas los textos del juego cambian a la fuente elegida)
    public void setDislexiaMode(System.Boolean dislexiaMode)
    {
        TextAccesibilityManager.Instance.setDislexiaMode(dislexiaMode);

        serializeToJSONToggle();
    }

    // Cambia el tamanyo de los textos
    public void onFontSizeChanged(System.Single value)
    {
        TextAccesibilityManager.Instance.onFontSizeChanged(value);

        serializeToJSONFontSize(value);
    }
}
