using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static HUDCustom;

public class HUDManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject HUDprefab;
    public float scaleFactor;
    public float shadowFactor;

    Dictionary<string, ObjectInfo> HUDInfo; // Custom HUD info

    public enum HUDTypes { defaultHUD, customHUD };

    HUDTypes currentSelection;

    static private HUDManager _instance;
    public static HUDManager Instance { get { return _instance; } }

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
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        HUDInfo = new Dictionary<string, HUDManager.ObjectInfo>();

        // Default values for later customization
        scaleFactor = 100.0f;
        shadowFactor = 0.0f;
        currentSelection = HUDTypes.defaultHUD;

        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 getCustomObjBaseScale(string objName)
    {
        return HUDInfo[objName].baseScale;
    }
    public GameObject getDefaultPrefab()
    {
        return HUDprefab;
    }
    public void saveConfig(HUDTypes type, Dictionary<string, HUDManager.ObjectInfo> objInfo = null, GameObject currHUD = null)
    {
        if(objInfo != null) { HUDInfo = objInfo; }
        if (currHUD == null) { currHUD = HUDprefab; }
        string totalInfo = "{\n";
        for (int i = 0; i < currHUD.transform.childCount; i++)
        {
            Transform tr = currHUD.transform.GetChild(i);
            ObjectInfo aux = HUDInfo[tr.gameObject.name];
            aux.scaleFactor = scaleFactor;
            aux.shadowFactor = shadowFactor;
            aux.pos = tr.GetComponent<RectTransform>().anchoredPosition3D;
            HUDInfo[tr.gameObject.name] = aux;
            string info = JsonUtility.ToJson(aux);
            if (i == currHUD.transform.childCount - 1)
            {
                totalInfo += "\"" + tr.gameObject.name + "\":" + info + "\n";

            }
            else
            {
                totalInfo += "\"" + tr.gameObject.name + "\":" + info + ",\n";
            }
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/"+ type.ToString()+".json", totalInfo + " \n}");

        //prueba de lectura json 
        //GameObject aux2 = Instantiate(defaultPrefab, panel.transform);
        //customHUD.SetActive(false); defaultHUD.SetActive(false);
        //applyCustomToHUDPrefab(ref aux2);
    }
    public bool applySavedConfigToHUD(GameObject HUDObject, HUDTypes type) // Default or custom
    {
        string path = Path.Combine(Application.persistentDataPath, type.ToString()+".json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            foreach (Transform tr in HUDObject.transform)
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
                    if (type == HUDTypes.customHUD)
                    {
                        HUDInfo[tr.gameObject.name] = data; //shadow spread entre 1.5?
                        scaleFactor = data.scaleFactor;
                        shadowFactor = data.shadowFactor;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"{type.ToString()} HUD no guardado en {path}, se mantiene el default");
            return false;
        }
        return true;
    }

    public void SetCurrentHUDType(HUDTypes type)
    {
        currentSelection = type;
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

    /// <summary>
    /// Load HUD segun selección del player en Menú
    /// </summary>
    void loadCurrentHUD()
    {
        applySavedConfigToHUD(HUDprefab, currentSelection); // Default or custom
    }
}
