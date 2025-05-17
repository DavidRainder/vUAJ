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
        AccesibilityManager.Instance.fontSizeChange += changeSize;
        AccesibilityManager.Instance.fontChange += changeFont;

        text = gameObject.GetComponent<TextMeshProUGUI>();
        if (text)
        {
            OGfont = text.font;
            OGfontsize = text.fontSize;
            if(AccesibilityManager.Instance.applyFonts) text.font = AccesibilityManager.Instance.currentFontAsset;
            text.fontSize = text.fontSize * AccesibilityManager.Instance.currentFontSize;
            text.ForceMeshUpdate(false, true);
        }
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