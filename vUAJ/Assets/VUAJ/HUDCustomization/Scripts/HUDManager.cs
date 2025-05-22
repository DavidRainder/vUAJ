using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Tooltip("Contiene el prefab del HUD a instanciar y serializa las modificaciones de un HUD customizable." +
    "Aplica cambios sobre HUDS y los guarda.")]
public class HUDManager : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab del HUD in game para visualización de la customización y valores por defecto")]
    private GameObject HUDprefab;

    [Tooltip("Factor de escalado de los items del HUD customizado")]
    public float scaleFactor;

    [Tooltip("Factor de sombreado de los items del HUD customizado")]
    public float shadowFactor;

    Dictionary<string, ObjectInfo> HUDInfo; // Información sobre los cambios de HUD para serialización (ha perdido relevancia)

    [Tooltip("Tipos de HUD")]
    public enum HUDTypes { defaultHUD, customHUD };

    /// <summary>
    /// Tipo HUD a instanciarse en el juego seleccionado en el momento
    /// </summary>
    HUDTypes currentSelection; 

    static private HUDManager _instance;
    public static HUDManager Instance { get { return _instance; } }

    /// <summary>
    /// Estrctura que habilita el guardado de modificaciones en el HUD. Contiene información sobre el tamaño, posición y sombra.
    /// </summary>
    public struct ObjectInfo
    {
        public Vector3 baseScale; // Escala base antes de sufrir modificación alguna
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

        // Valores default para la customización posterior
        scaleFactor = 1.0f;
        shadowFactor = 0.0f;
        currentSelection = HUDTypes.defaultHUD;
        enableShadows(); 

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Devuelve la escala original del objeto previa a modificaciones.
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public Vector3 getCustomObjBaseScale(string objName)
    {
        return HUDInfo[objName].baseScale;
    }

    /// <summary>
    /// Devuelve el prefab original de HUD
    /// </summary>
    /// <returns></returns>
    public GameObject getDefaultPrefab()
    {
        return HUDprefab;
    }
    /// <summary>
    /// Añade el componente de sombra y valores default a todos los objetos del HUD si no lo poseen de antemano
    /// </summary>
    public void enableShadows() 
    {
        foreach (Transform tr in HUDprefab.transform)
        {
            UnityEngine.UI.DropShadow shadow = tr.gameObject.GetComponent<UnityEngine.UI.DropShadow>();
            if (shadow == null)
            {
                shadow = tr.gameObject.AddComponent<UnityEngine.UI.DropShadow>();

                shadow.ShadowSpread = new Vector2(0.0f, 0.0f);
                shadow.effectColor = Color.black;
                shadow.EffectDistance = new Vector2(0f, 0f);
                shadow.iterations = 50;
            }
        }
    }

    /// <summary>
    /// Serializa y guarda en un JSON los datos de todos los objetos de un HUD.
    /// </summary>
    /// <param name="type">Tipo de HUD (Custom, Default)</param>
    /// <param name="objInfo">Información previa para sobreescribir valores del manager</param>
    /// <param name="currHUD">Objeto HUD que guardar</param>
    public void saveConfig(HUDTypes type, Dictionary<string, HUDManager.ObjectInfo> objInfo = null, GameObject currHUD = null)
    {
        if(objInfo != null) { HUDInfo = objInfo; }
        if (currHUD == null) { currHUD = HUDprefab; } // Si no guardamos un objeto específico, guardaremos los valores del default

        // Serialización y actualización de valores
        string totalInfo = "{\n";
        for (int i = 0; i < currHUD.transform.childCount; i++)
        {
            Transform tr = currHUD.transform.GetChild(i);
            ObjectInfo aux = HUDInfo[tr.gameObject.name];

            if(type == HUDTypes.customHUD)
            {
                aux.scaleFactor = scaleFactor;
                aux.shadowFactor = shadowFactor;
            }
            else 
            {
                // Si es default no guardamos modificaciones
                aux.scaleFactor = 1.0f;
                aux.shadowFactor = 0.0f;
            }

            // Actualización de la posición a guardar
            aux.pos = tr.GetComponent<RectTransform>().anchoredPosition3D;
            HUDInfo[tr.gameObject.name] = aux;

            // Conversión del struct y guardado en el JSON del tipo de HUD
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
    }

    /// <summary>
    /// Deserializa el HUD contenido en un JSON y lo aplica a un objeto HUD
    /// </summary>
    /// <param name="HUDObject">Objeto HUD a modificar</param>
    /// <param name="type">Tipo de HUD (Custom, Default)</param>
    /// <returns></returns>
    public bool applySavedConfigToHUD(GameObject HUDObject, HUDTypes type) 
    {
        //Comprobamos que existe el archivo de guardado
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

                    // Actualizaciones de posición, tamaño y sombras
                    rect.anchoredPosition3D = data.pos;
                    rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y, 0);
                    rect.localScale = data.baseScale * data.scaleFactor;
                    UnityEngine.UI.DropShadow shadow = tr.gameObject.GetComponent<UnityEngine.UI.DropShadow>();
                    if(shadow != null)
                    {
                        shadow.effectColor = Color.black;
                        shadow.ShadowSpread = new Vector2(data.shadowFactor * 1.5f, data.shadowFactor * 1.5f);
                    }
                    if (type == HUDTypes.customHUD)
                    {
                        HUDInfo[tr.gameObject.name] = data; // Guardo las modificaciones actuales 
                        scaleFactor = data.scaleFactor;
                        shadowFactor = data.shadowFactor;
                    }
                }
            }
        }
        else
        {
            if(type == HUDTypes.customHUD)
            {
                Debug.LogWarning($"{type.ToString()} HUD no guardado en {path}, se mantiene el default"); // Si el prefab de HUD asignado está bien no causará ningún fallo
                applySavedConfigToHUD(HUDObject, HUDTypes.defaultHUD);
            } else
            {
                Debug.LogWarning($"Default settings guardadas incorrectamente en {path}, no se pudo aplicar al HUD"); // Si el prefab de HUD asignado está bien no causará ningún fallo
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Establece la selección de HUD actual (a instanciarse en el juego).
    /// </summary>
    /// <param name="type"></param>
    public void SetCurrentHUDType(HUDTypes type)
    {
        currentSelection = type;
    }

    /// <summary>
    /// Método auxiliar para el encuentro de claves y extracción de objetos de un guardado en JSON
    /// </summary>
    /// <param name="fullJson"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    string ExtractJsonObject(string fullJson, string key) // A simplificar si se incorpora algo adicional a JSONUtility
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
    public void loadCurrentHUD(GameObject obj)
    {
        applySavedConfigToHUD(obj, currentSelection); // Default or custom
    }
}
