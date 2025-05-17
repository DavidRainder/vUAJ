using TMPro;
using UnityEngine;

public class FontOptions : MonoBehaviour
{
    TMP_Text text;
    TMP_FontAsset OGfont;
    float OGfontsize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextAccesibilityManager.Instance.fontSizeChange += changeSize;
        TextAccesibilityManager.Instance.fontChange += changeFont;

        text = gameObject.GetComponent<TextMeshProUGUI>();
        if (text)
        {
            OGfont = text.font;
            OGfontsize = text.fontSize;
            if(TextAccesibilityManager.Instance.applyFonts) text.font = TextAccesibilityManager.Instance.currentFontAsset;
            text.fontSize = text.fontSize * TextAccesibilityManager.Instance.currentFontSize;
            text.ForceMeshUpdate(false, true);
        }
        else Debug.LogWarning("The object does not have a TextMeshPro component so the script doesnt have an effect");
    }

    void changeFont(TMP_FontAsset font)
    {
        if (text != null)
        {
            if (font != null) text.font = font;
            else text.font = OGfont;
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