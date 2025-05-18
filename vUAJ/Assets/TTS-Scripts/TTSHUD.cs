using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TTSHUD : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    TMP_Text UIText= null;
    [SerializeField]
    string fieldText = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIText != null)
            TTSManager.Instance.StartSpeech(UIText.text);
        else if (fieldText != null)
            TTSManager.Instance.StartSpeech(fieldText);
        else Debug.LogWarning("The UI element does not have Text associated so the script doesnt have an effect");
    }
}
