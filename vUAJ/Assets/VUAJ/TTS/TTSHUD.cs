using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Componente que se ha de poner a todos los elementos que se quiera que tengan audiodescripcion
// asociada al clickar en ellos
public class TTSHUD : MonoBehaviour, IPointerClickHandler
{
    // Si se quiere que se lea lo que pone en el componente de texto
    [SerializeField]
    TMP_Text UIText= null;
    // Si se quiere que se lea una descripcion metida a mano desde el editor
    [SerializeField]
    string fieldText = null;

    // Lee la descripcion al ser clickado
    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIText != null)
            TTSManager.Instance.StartSpeech(UIText.text);
        else if (fieldText != null)
            TTSManager.Instance.StartSpeech(fieldText);
        else Debug.LogWarning("The UI element does not have Text associated so the script doesnt have an effect");
    }
}
