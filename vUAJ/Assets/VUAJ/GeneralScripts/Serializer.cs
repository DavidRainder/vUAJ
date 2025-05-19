using System.IO;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="folder"> carpeta. se le añade Application.persistenDataPath </param>
    /// <param name="fileName"></param>
    public void WriteToJSON(string fileName)
    {
        info = info.TrimEnd(',');
        writeToJSON(Application.persistentDataPath + "/" + fileName);
    }

    public void WriteToJSON(string folder, string fileName)
    {
        info = info.TrimEnd(',');
        Directory.CreateDirectory(Application.persistentDataPath + "/" + folder);
        writeToJSON(Application.persistentDataPath + "/" + folder + "/" + fileName);
    }

    private void writeToJSON(string fullPath) 
    {
        info = info.TrimEnd(',');
        System.IO.File.WriteAllText(fullPath + ".json", info);
    }

    public int getFromJSONStruct<T>(string path, out T type) where T : struct
    {
        try
        {
            string newPath = Application.persistentDataPath + "/" + path + ".json";
            if (File.Exists(newPath))
            {
                string json = System.IO.File.ReadAllText(newPath);
                type = JsonUtility.FromJson<T>(json);
                return 1;
            }
            else {
                type = default(T);
                return -1;
            }
        }
        catch (FileNotFoundException e)
        {
            type = default(T);
            return -1;
        }
    }
}
