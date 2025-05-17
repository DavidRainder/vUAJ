using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FontDropdown : MonoBehaviour
{
    [SerializeField]
    TMP_FontAsset[] fonts;
    TMP_Dropdown fontDropdown;

    private void Start()
    {
        fontDropdown = GetComponent<TMP_Dropdown>();

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
        if (index < fonts.Length && index >= 0)
        {
            AccesibilityManager.Instance.onFontChanged(fonts[index]);
        }
    }

}
