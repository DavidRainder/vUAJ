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

[Tooltip("Regenta el canvas y opciones de ajustes que permiten la customización de un HUD")]
public class HUDCustom : MonoBehaviour
{
    [SerializeField, Tooltip("Representación de la pantalla de juego. Fija los límites de movimiento de elementos.")]
    private GameObject panel;
    /// <summary>
    /// Prefab original del HUD para realizar las instancias de visualización
    /// </summary>
    private GameObject defaultPrefab;
    /// <summary>
    /// Objeto de visualización del HUD por defecto
    /// </summary>
    private GameObject defaultHUD;
    /// <summary>
    /// Objeto de visualización del HUD customizado
    /// </summary>
    private GameObject customHUD;
    [SerializeField, Tooltip("Opciones de selección de HUD")]
    private TMP_Dropdown presetDropdown;
    [SerializeField, Tooltip("Botón reset CustomHUD")]
    private Button resetCustomButton;
    [SerializeField, Tooltip("Slider tamaño CustomHUD")]
    private Slider shadowSlider;
    [SerializeField, Tooltip("Texto tamaño CustomHUD")]
    private TextMeshProUGUI shadowNumber;
    [SerializeField, Tooltip("Slider sombra CustomHUD")]
    private Slider scaleSlider;
    [SerializeField, Tooltip("Texto sombra CustomHUD")]
    private TextMeshProUGUI scaleNumber;
    [SerializeField, Tooltip("Conjunto de UI elements para CustomHUD")]
    private GameObject customSettings;
    [SerializeField, Tooltip("Visualización de valores del DefaultHUD en elementos no interactuables")] // Para efecto visual, no tienen recupercusión en código
    private GameObject defaultFakeSettings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Referencia prefab HUD
        defaultPrefab = HUDManager.Instance.getDefaultPrefab();

        // Instancias de los visualizadores de HUDS
            // Default
        defaultHUD = Instantiate(defaultPrefab, panel.transform); 
        Dictionary<string, HUDManager.ObjectInfo> HUDInfo = new Dictionary<string, HUDManager.ObjectInfo>();
        
        RectTransform overviewRect = panel.GetComponent<RectTransform>();
        RectTransform rect = defaultHUD.GetComponent<RectTransform>();

        foreach (Transform tr in defaultHUD.transform)
        {
            HUDInfo.Add(tr.name, new HUDManager.ObjectInfo (tr.localScale, 1, 0, tr.position)); // Valores iniciales default
            removeInteractableComp(tr); 
        } 

        // Guardado inicial en JSON del HUD default según prefab
        HUDManager.Instance.saveConfig(HUDManager.HUDTypes.defaultHUD, HUDInfo);

            // Custom 
        customHUD = Instantiate(defaultPrefab, panel.transform);
        
        // Load del HUD customizado según JSON en el equipo o por copia de default 
        applySavedToCustom(); 
        rect = customHUD.GetComponent<RectTransform>();
        foreach (Transform tr in customHUD.transform)
        { 
            tr.gameObject.AddComponent<Drag>(); // Habilita movmiento en panel
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            if (tr.gameObject.GetComponent<UnityEngine.UI.DropShadow>()== null)
            {
                UnityEngine.UI.DropShadow shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();

                shadow.ShadowSpread = new Vector2(0.0f, 0.0f);
                shadow.effectColor = Color.black;
                shadow.EffectDistance = new Vector2(0f, 0f);
            }
            removeInteractableComp(tr);
        }

        // Settings iniciales 
        HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.customHUD); // Se carga custom al entrar en menú 
       // HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.defaultHUD); 
        defaultHUD.SetActive(false);
        defaultFakeSettings.SetActive(false);
    }

    /// <summary>
    /// Deshabilita los componentes que puedan ocasionar interacción y dificultar el movimiento de componentes
    /// </summary>
    /// <param name="tr"></param>
    void removeInteractableComp(Transform tr)
    {
        // De momento deshabilita los botones y sliders contenidos
        Button [] button; 
        if ((button=tr.gameObject.GetComponentsInChildren<Button>()) != null) {
            for (int i = 0; i < button.Length; i++)
            {
                button[i].interactable = false;
            }
        }
        Slider[] slider;
        if ((slider = tr.gameObject.GetComponentsInChildren<Slider>()) != null)
        {
            for (int i = 0; i < slider.Length; i++)
            {
                slider[i].interactable = false;
            }
        }
    }
    /// <summary>
    /// Guardado de la customización actual
    /// </summary>
    public void saveCustom()
    {
        HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.customHUD);
        HUDManager.Instance.saveConfig(HUDManager.HUDTypes.customHUD, null, customHUD);
    }

    /// <summary>
    /// Actualiza valores y llama a la modificación de tamaños de elementos del HUD
    /// </summary>
    /// <param name="value"></param>
    public void sliderScale(System.Single value)
    {
        scaleNumber.text = ((int)scaleSlider.value).ToString() + "%";
        scaleItems(scaleSlider.value);
        HUDManager.Instance.scaleFactor = scaleSlider.value / 100;

    }

    /// <summary>
    /// Modifica el tamaño de todos los items del HUD customizable
    /// </summary>
    /// <param name="scale"></param>
    void scaleItems(float scale)
    {
        foreach (Transform tr in customHUD.transform)
        {
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            rectChild.localScale = HUDManager.Instance.getCustomObjBaseScale(tr.gameObject.name) * scale/100;
            rectChild.ForceUpdateRectTransforms();
        }

    }
    /// <summary>
    /// Actualiza valores y llama a la modificación de sombras de elementos del HUD
    /// </summary>
    /// <param name="value"></param>
    public void sliderShadow(System.Single value)
    {
        shadowNumber.text = ((int)shadowSlider.value).ToString() + "%";
        setShadows(shadowSlider.value);
        HUDManager.Instance.shadowFactor = shadowSlider.value;
    }

    /// <summary>
    /// Settea la sombra de todos los elementos del HUD customizable
    /// </summary>
    /// <param name="intensity"></param>
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

    /// <summary>
    /// Cambio de HUD seleccionado
    /// </summary>
    public void changePreset()
    {
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
            defaultFakeSettings.SetActive(true); // Para visualización
            HUDManager.Instance.SetCurrentHUDType(HUDManager.HUDTypes.defaultHUD);
        }
    }

    /// <summary>
    /// Comprueba si existe un guardado del custom HUD y lo carga. Actualiza las herramientas de UI asociadas.
    /// </summary>
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
        // Listeners
        scaleSlider.onValueChanged.AddListener(sliderScale);
        shadowSlider.onValueChanged.AddListener(sliderShadow);
    }

    /// <summary>
    /// Reset del HUD customizable a último guardado (o default)
    /// </summary>
    public void resetCustom()
    {
       applySavedToCustom();
    }

    /// <summary>
    /// Inicialización de sliders y textos asociados
    /// </summary>
    public void initSliders()
    {
        shadowSlider.value = HUDManager.Instance.shadowFactor; 
        scaleSlider.value = HUDManager.Instance.scaleFactor * 100;
        shadowNumber.text = (int)shadowSlider.value + "%"; scaleNumber.text = (int)scaleSlider.value + "%";
    }
}
