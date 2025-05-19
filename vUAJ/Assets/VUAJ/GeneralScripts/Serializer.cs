using UnityEngine;

public class Serializer : MonoBehaviour
{
    string info;

    public void Clear()
    {
        info = string.Empty;
    }

    public void Serialize(object obj)
    {
        info += JsonUtility.ToJson(obj, true) + ",";
    }

    public void WriteToJSON(string path)
    {
        info = info.TrimEnd(',');
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + path + ".json", "{\n" + "\"" + gameObject.name + "\":[" +  info + "]\n}");
    }

    public object getFromJSON(string path, System.Type type)
    {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + path + ".json");
        return JsonUtility.FromJson(json, type);
    }
}
