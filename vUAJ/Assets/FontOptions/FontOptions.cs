using TMPro;
using UnityEngine;

public class FontOptions : MonoBehaviour
{
    TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AccesibilityManager.Instance.fontSizeChange += changeSize;
        AccesibilityManager.Instance.fontChange += changeFont;

        text = gameObject.GetComponent<TMP_Text>();
        if (text)
        {
            text.font = AccesibilityManager.Instance.currentFontAsset;
            text.fontSize = AccesibilityManager.Instance.currentFontSize;
            //text.ForceMeshUpdate(false, true);
        }
    }

    void changeFont(TMP_FontAsset font)
    {
        var textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.font = font;
        }
        else Debug.LogWarning("The object does not have a TextMeshPro component so the script doesnt have an effect");
    }

    void changeSize(float size)
    {
        var textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.fontSize = size;
        }
        else Debug.LogWarning("The object does not have a TextMeshPro component so the script doesnt have an effect");
    }
}
