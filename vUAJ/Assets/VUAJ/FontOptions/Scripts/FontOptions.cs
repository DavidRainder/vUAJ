using TMPro;
using UnityEngine;

// Componente que se ha de poner a todos los textos que se quiera que puedan ser afectados en
// tamanyo y fuente cuando se cambian estas opciones en el menu de accesibilidad de texto
public class FontOptions : MonoBehaviour
{
    // Refencia al componente de texto
    TMP_Text text = null;
    // Fuente original
    TMP_FontAsset OGfont;
    // Tamanyo original
    float OGfontsize;

    // Se suscribe a los eventos de cambio de fuente y de tamanyo, y se actualiza si estos
    // han cambiado en el menu
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        TextAccesibilityManager.Instance.fontSizeChange += changeSize;
        TextAccesibilityManager.Instance.fontChange += changeFont;

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

    // Cambia la fuente del texto a la proporcionada
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

    // Cambia el tamanyo del texto (x1, x1.25, x1.5, x1.75) segun el float proporcionado
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