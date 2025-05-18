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
    [SerializeField]
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
    Dictionary<string, ObjectInfo> HUDInfo;

    float currentIconScale;
    float currentShadowValue;


    public struct ObjectInfo
    {
        public Vector3 baseScale;
        public float scaleFactor;
        public float shadowFactor;
        public Vector3 pos;
        public ObjectInfo(Vector3 scale, float scaleF, float shadow, Vector3 position)
        {
            baseScale = scale;
            scaleFactor = scaleF;
            shadowFactor = shadow;
            pos = position;
        }
    }
    void Start()
    {
        HUDInfo = new Dictionary<string, ObjectInfo>();
        // Instantiate HUD prefab visualizer 
        defaultHUD = Instantiate(defaultPrefab, panel.transform); 
        
        RectTransform overviewRect = panel.GetComponent<RectTransform>();
        RectTransform rect = defaultHUD.GetComponent<RectTransform>();

        foreach (Transform tr in defaultHUD.transform)
        {
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            UnityEngine.UI.DropShadow shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();

            shadow.ShadowSpread = new Vector2(0.0f, 0.0f);
            shadow.effectColor = Color.black;
            shadow.EffectDistance = new Vector2(0f, 0f);
            // rectChild.ForceUpdateRectTransforms();
            HUDInfo.Add(tr.gameObject.name, new ObjectInfo(tr.localScale, 1, 0, rectChild.anchoredPosition3D));
        }

        customHUD = Instantiate(defaultPrefab, panel.transform);
        rect = customHUD.GetComponent<RectTransform>();
        foreach (Transform tr in customHUD.transform)
        { 
            tr.gameObject.AddComponent<Drag>();
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            UnityEngine.UI.DropShadow shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();

            shadow.ShadowSpread = new Vector2(0.0f, 0.0f);
            shadow.effectColor = Color.black;
            shadow.EffectDistance = new Vector2(0f, 0f);
            // rectChild.ForceUpdateRectTransforms();
        }
        defaultHUD.SetActive(false);
        defaultFakeSettings.SetActive(false);
        // Listeners
        //presetDropdown.onValueChanged.AddListener(changePreset);
        scaleSlider.onValueChanged.AddListener(sliderScale);
        shadowSlider.onValueChanged.AddListener(sliderShadow);
    }

    public void saveConfig()
    {
        string totalInfo = "{\n"; 
            for (int i = 0; i<customHUD.transform.childCount; i++)
            {
                Transform tr = customHUD.transform.GetChild(i);
                ObjectInfo aux = HUDInfo[tr.gameObject.name]; 
                aux.scaleFactor = scaleSlider.value / 100;
                aux.shadowFactor = shadowSlider.value;
                aux.pos = tr.GetComponent<RectTransform>().anchoredPosition3D;  
                HUDInfo[tr.gameObject.name] = aux;
                string info = JsonUtility.ToJson(aux);
                if (i == customHUD.transform.childCount - 1)
                {
                   totalInfo += "\"" + tr.gameObject.name + "\":" + info + "\n";
                    
                }
                else
                {
                totalInfo += "\"" + tr.gameObject.name + "\":" + info + ",\n";
                }
            }
            System.IO.File.WriteAllText(Application.persistentDataPath + "/customHUD.json",totalInfo+ " \n}");

        //prueba de lectura json 
        //GameObject aux2 = Instantiate(defaultPrefab, panel.transform);
        //customHUD.SetActive(false); defaultHUD.SetActive(false);
        //applyCustomToHUDPrefab(ref aux2);
    }

    public void sliderScale(System.Single value)
    {
        scaleNumber.text = scaleSlider.value.ToString() + "%";
        scaleItems(scaleSlider.value);
    }
    void scaleItems(float scale)
    {
        foreach (Transform tr in customHUD.transform)
        {
            //child.GetComponent<RectTransform>().localScale *= 0.5f;
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            rectChild.localScale = HUDInfo[tr.gameObject.name].baseScale * scale/100;
            Debug.Log(scale);
            Debug.Log(rectChild.localScale);
            rectChild.ForceUpdateRectTransforms();
        }

    }
    public void sliderShadow(System.Single value)
    {
        shadowNumber.text = shadowSlider.value.ToString() + "%";
        setShadows(shadowSlider.value);
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
        }
        else
        {
            customHUD.SetActive(false);
            defaultHUD.SetActive(true);
            customSettings.SetActive(false);
            defaultFakeSettings.SetActive(true);
        }
    }
    public void resetCustom()
    {
        foreach (Transform tr in customHUD.transform)
        {
            RectTransform rectChild = tr.gameObject.GetComponent<RectTransform>();
            rectChild.anchoredPosition3D = HUDInfo[tr.gameObject.name].pos;
        }
        shadowSlider.value = shadowSlider.minValue; scaleSlider.value = 100; 
        shadowNumber.text = shadowSlider.value + "%"; scaleNumber.text = 100+ "%";
    }
    string ExtractJsonObject(string fullJson, string key)
    {
        int keyIndex = fullJson.IndexOf($"\"{key}\"");
        if (keyIndex == -1) return null;

        int braceStart = fullJson.IndexOf('{', keyIndex);
        if (braceStart == -1) return null;

        int braceCount = 0;
        for (int i = braceStart; i < fullJson.Length; i++)
        {
            if (fullJson[i] == '{') braceCount++;
            else if (fullJson[i] == '}') braceCount--;

            if (braceCount == 0)
            {
                int braceEnd = i;
                return fullJson.Substring(braceStart, braceEnd - braceStart + 1);
            }
        }

        return null;
    }
    public void applyCustomToHUDPrefab(ref GameObject HUDPrefab)
    {
        string path = Path.Combine(Application.persistentDataPath, "customHUD.json");
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            foreach (Transform tr in HUDPrefab.transform)
            {
                string obj = ExtractJsonObject(json, tr.gameObject.name);

                if (!string.IsNullOrEmpty(obj))
                {
                    ObjectInfo data = JsonUtility.FromJson<ObjectInfo>(obj);
                    RectTransform rect = tr.GetComponent<RectTransform>();
                    rect.anchoredPosition3D = data.pos;
                    rect.localScale = data.baseScale * data.scaleFactor;
                    UnityEngine.UI.DropShadow shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();
                    shadow.effectColor = Color.black;
                    shadow.ShadowSpread = new Vector2(data.shadowFactor * 1.5f, data.shadowFactor * 1.5f);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Custom HUD no guardado en {path}, se mantiene el default");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
