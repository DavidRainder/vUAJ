using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class JSONParser : MonoBehaviour
{
    [Serializable]
    private class StringList
    {
        public List<string> Subtitles; //Same name as identifier in JSON

    }

    public static List<string> FromJson(string file) {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + file);
        StringList l = JsonUtility.FromJson<StringList>(json);
        return l.Subtitles;
    
    }
}