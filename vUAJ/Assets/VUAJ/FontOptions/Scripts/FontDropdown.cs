using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Componente que tiene el dropdown del menu de opciones de accesibilidad de texto y permite
// escoger una fuente entre una lista proporcionada de fuentes aptas para dislexia
public class FontDropdown : MonoBehaviour
{
    // Lista de opciones de fuentes
    [SerializeField]
    TMP_FontAsset[] fonts;
    // Referencia al propio dropdown 
    TMP_Dropdown fontDropdown;

    // Crea las opciones del dropdown con las fuentes proporcionadas
    private void Start()
    {
        fontDropdown = GetComponent<TMP_Dropdown>();

        fontDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < fonts.Length; i++)
        {
            string fontName = fonts[i].name;

            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(fontName);
            options.Add(option);
        }
        fontDropdown.AddOptions(options);
    }

    // Avisa al manager del cambio de fuente
    public void UpdateUIFont(int index)
    {
        if (index < fonts.Length && index >= 0)
        {
            TextAccesibilityManager.Instance.onFontChanged(fonts[index]);
        }
    }

}
