using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Componente que tiene el dropdown del menu de opciones de accesibilidad de texto y permite
// escoger una fuente entre una lista proporcionada de fuentes aptas para dislexia
public class FontDropdown : MonoBehaviour
{
    // Lista de opciones de fuentes
    [SerializeField]
    TMP_FontAsset[] fonts;
    // Referencia al propio dropdown 
    TMP_Dropdown fontDropdown;

    // Referencia al script de guardado
    Serializer serializer;

    [System.Serializable]
    struct DropdownSelectionSerializedInfo
    {
        public int value;
    }

    // Crea las opciones del dropdown con las fuentes proporcionadas
    private void Start()
    {
        serializer = GetComponent<Serializer>();

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

        if (serializer.getFromJSONStruct("SettingsMenu/"+gameObject.name, out DropdownSelectionSerializedInfo info) != -1) 
        {
            fontDropdown.value = info.value;   
        }
    }

    void serializeDropdownInfo()
    {
        serializer.Clear();
        var dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown != null)
        {
            DropdownSelectionSerializedInfo dropdownInfo = new DropdownSelectionSerializedInfo();
            dropdownInfo.value = dropdown.value;
            serializer.Serialize(dropdownInfo);
            serializer.WriteToJSON("SettingsMenu", gameObject.name);
        }
        else Debug.LogWarning("El objeto no tiene asociado un componente Dropdown");
    }

    // Avisa al manager del cambio de fuente
    public void UpdateUIFont(int index)
    {
        if (index < fonts.Length && index >= 0)
        {
            TextAccesibilityManager.Instance.onFontChanged(fonts[index]);
        }

        serializeDropdownInfo();
    }
}
