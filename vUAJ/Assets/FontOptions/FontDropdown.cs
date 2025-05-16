using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontDropdown : MonoBehaviour
{
    [SerializeField]
    TMP_FontAsset[] fonts;
    TMP_Dropdown fontDropdown;

    private void Start()
    {
        fontDropdown = GetComponent<TMP_Dropdown>();

        //Clear the dropdown just in case
        fontDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < fonts.Length; i++)
        {
            //<Font=FontAssetName> Whatever text </font>
            string fontName = fonts[i].name;

            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(fontName);
            options.Add(option);
        }
        fontDropdown.AddOptions(options);
    }

    public void UpdateUIFont(int index)
    {
        //Just to be safe the index specified is within range
        if (index < fonts.Length && index >= 0)
        {
            AccesibilityManager.Instance.onFontChanged(fonts[index]);
        }
    }

}
