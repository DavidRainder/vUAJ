using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HUDCustom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject panel;
    private GameObject defaultPrefab;
    private GameObject defaultHUD;
    private GameObject customHUD;
    [SerializeField]
    private TMP_Dropdown presetDropdown;
    [SerializeField]
    private Button resetCustomButton;
    [SerializeField]
    private Slider shadowSlider;
    [SerializeField]
    private TextMeshProUGUI shadowNumber;
    [SerializeField]
    private Slider scaleSlider;
    [SerializeField]
    private TextMeshProUGUI scaleNumber;
    [SerializeField]
    private GameObject customSettings;
    [SerializeField]
    private GameObject defaultFakeSettings;

    void Start()
    {
        // Referencia HUD
        defaultPrefab = HUDManager.Instance.getDefaultPrefab();

        // Instancia HUD prefab visualizer 
            // Default
        Dictionary<string, HUDManager.ObjectInfo> HUDInfo = new Dictionary<string, HUDManager.ObjectInfo>();
        defaultHUD = Instantiate(defaultPrefab, panel.transform); 
        
        RectTransform overviewRect = panel.GetComponent<RectTransform>();
        RectTransform rect = defaultHUD.GetComponent<RectTransform>();
        foreach (Transform tr in defaultHUD.transform)
        {
            HUDInfo.Add(tr.name, new HUDManager.ObjectInfo (tr.localScale, 1, 0, tr.position));
        }
        HUDManager.Instance.saveConfig(HUDManager.HUDTypes.defaultHUD, HUDInfo); // Save


            // Custom 
        customHUD = Instantiate(defaultPrefab, panel.transform);
        applySavedToCustom(); // Load
;
        rect = customHUD.GetComponent<RectTransform>();
        foreach (Transform tr in customHUD.transform)
        { 
            tr.gameObject.AddComponent<Drag>();
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            if (tr.gameObject.GetComponent<UnityEngine.UI.DropShadow>()== null)
            {
                UnityEngine.UI.DropShadow shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();

                shadow.ShadowSpread = new Vector2(0.0f, 0.0f);
                shadow.effectColor = Color.black;
                shadow.EffectDistance = new Vector2(0f, 0f);
            }
        }

        // Settings iniciales 
        HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.customHUD); // Se carga custom al entrar en menú 
       // HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.defaultHUD); 
        defaultHUD.SetActive(false);
        defaultFakeSettings.SetActive(false);
    }
    public void saveCustom()
    {
        HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.customHUD);
        HUDManager.Instance.saveConfig(HUDManager.HUDTypes.customHUD, null, customHUD);
    }
    public void sliderScale(System.Single value)
    {
        scaleNumber.text = ((int)scaleSlider.value).ToString() + "%";
        scaleItems(scaleSlider.value);
        HUDManager.Instance.scaleFactor = scaleSlider.value / 100;

    }
    void scaleItems(float scale)
    {
        foreach (Transform tr in customHUD.transform)
        {
            //child.GetComponent<RectTransform>().localScale *= 0.5f;
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            rectChild.localScale = HUDManager.Instance.getCustomObjBaseScale(tr.gameObject.name) * scale/100;
            rectChild.ForceUpdateRectTransforms();
        }

    }
    public void sliderShadow(System.Single value)
    {
        shadowNumber.text = ((int)shadowSlider.value).ToString() + "%";
        setShadows(shadowSlider.value);
        HUDManager.Instance.shadowFactor = shadowSlider.value;
    }
    void setShadows(float intensity)
    {
        foreach (Transform tr in customHUD.transform)
        {
            UnityEngine.UI.DropShadow shadow = tr.gameObject.GetComponent<UnityEngine.UI.DropShadow>();

            shadow.ShadowSpread = new Vector2(1.5f* intensity, 1.5f* intensity);
            shadow.effectColor = Color.black;
            shadow.EffectDistance = new Vector2(0f, 0f);
        }
    }
    void DrawRect(RectTransform rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0.01f), new Vector3(rect.rect.width, rect.rect.height, 0.01f));
    }
    public void changePreset()
    {
        Debug.Log("hiii");
        if (presetDropdown.value == 0)
        {
            customHUD.SetActive(true);
            defaultHUD.SetActive(false);
            customSettings.SetActive(true);
            defaultFakeSettings.SetActive(false);
            HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.customHUD);
        }
        else
        {
            customHUD.SetActive(false);
            defaultHUD.SetActive(true);
            customSettings.SetActive(false);
            defaultFakeSettings.SetActive(true);
            HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.defaultHUD);

        }
    }
    void applySavedToCustom()
    {
        if (!HUDManager.Instance.applySavedConfigToHUD(customHUD, HUDManager.HUDTypes.customHUD))
        {
            Debug.Log("Custom settings not found, loading default as base :)");
        }
        else
        {
            initSliders(); // Set sliders a valor correspondiente
        }
    }
    public void resetCustom()
    {
       applySavedToCustom();
    }

    public void initSliders()
    {
        shadowSlider.value = HUDManager.Instance.shadowFactor; 
        scaleSlider.value = HUDManager.Instance.scaleFactor * 100;
        shadowNumber.text = (int)shadowSlider.value + "%"; scaleNumber.text = (int)scaleSlider.value + "%";

        // Listeners
        scaleSlider.onValueChanged.AddListener(sliderScale);
        shadowSlider.onValueChanged.AddListener(sliderShadow);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
