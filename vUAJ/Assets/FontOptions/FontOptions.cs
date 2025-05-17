using TMPro;
using UnityEngine;

public class FontOptions : MonoBehaviour
{
    TMP_Text text;
    float OGfontsize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AccesibilityManager.Instance.fontSizeChange += changeSize;
        AccesibilityManager.Instance.fontChange += changeFont;

        text = gameObject.GetComponent<TextMeshProUGUI>();
        if (text)
        {
            OGfontsize = text.fontSize;
            text.font = AccesibilityManager.Instance.currentFontAsset;
            text.fontSize = text.fontSize * AccesibilityManager.Instance.currentFontSize;
            text.ForceMeshUpdate(false, true);
        }
    }

    void changeFont(TMP_FontAsset font)
    {
        if (text != null)
        {
            text.font = font;
            text.ForceMeshUpdate(false, true);
        }
        else Debug.LogWarning("The object does not have a TextMeshPro component so the script doesnt have an effect");
    }

    void changeSize(float size)
    {
        if (text != null)
        {
            text.fontSize = OGfontsize * size;
            text.ForceMeshUpdate(false, true);
        }
        else Debug.LogWarning("The object does not have a TextMeshPro component so the script doesnt have an effect");
    }
}
