using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Componente que tienen distintos elementos del HUD del menu de accesibilidad para
// llamar a distintas funciones de los managers
public class SettingFunctions : MonoBehaviour
{
    // Referencia al script de guardado
    Serializer serializer;

    private void Start()
    {
        serializer = GetComponent<Serializer>();
        if (gameObject.GetComponent<Toggle>() != null)
        {
            object toggle = serializer.getFromJSON(gameObject.name, typeof(Toggle));
        }
    }

    // Activa o desactiva el modo TTS (Text To Speech)
    public void setTTSMode()
    {
        TTSManager.Instance.TTSActivation();

        serializer.Clear();
        var toggle = GetComponent<Toggle>();
        if (toggle != null) serializer.Serialize(this.GetComponent<Toggle>());
        else Debug.LogWarning("El objeto no tiene asociado un componente Toggle");
        serializer.WriteToJSON(gameObject.name);
    }

    // Activa o desactiva el modo Volume Perception (colliders resaltados con una malla de color)
    public void setShowColliders(System.Boolean show)
    {
        VolumePerceptionManager.Instance.setShowColliders(show);

        serializer.Clear();
        var toggle = GetComponent<Toggle>();
        if (toggle != null) serializer.Serialize(this.GetComponent<Toggle>());
        else Debug.LogWarning("El objeto no tiene asociado un componente Toggle");
            serializer.WriteToJSON(gameObject.name);
    }

    // Activa o desactiva el modo dislexia friendly (todas los textos del juego cambian a la fuente elegida)
    public void setDislexiaMode(System.Boolean dislexiaMode)
    {
        TextAccesibilityManager.Instance.setDislexiaMode(dislexiaMode);

        serializer.Clear();
        var toggle = GetComponent<Toggle>();
        if (toggle != null) serializer.Serialize(this.GetComponent<Toggle>());
        else Debug.LogWarning("El objeto no tiene asociado un componente Toggle");
        serializer.WriteToJSON(gameObject.name);
    }

    // Cambia el tamanyo de los textos
    public void onFontSizeChanged(System.Single value)
    {
        TextAccesibilityManager.Instance.onFontSizeChanged(value);

        serializer.Clear();
        var button = GetComponent<Button>();
        if (button != null) serializer.Serialize(value);
        else Debug.LogWarning("El objeto no tiene asociado un componente Button");
        serializer.WriteToJSON("ButtonSize");
    }
}
