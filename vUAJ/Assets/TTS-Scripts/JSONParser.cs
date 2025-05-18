using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//No hace falta incluir esta clase en un objeto en escena
//Se encarga del parseo de elementos de JSON para que lo pueda usar TTSSubtitles
public class JSONParser : MonoBehaviour
{
    [Serializable]
    private class StringList
    {
        public List<string> Subtitles; //El nombre de esta variable debe ser el mismo que el identificador dentro de JSON

    }

    public static List<string> FromJson(string file) {
        string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/" + file);
        if (json == null) return null;
        StringList l = JsonUtility.FromJson<StringList>(json);
        return l.Subtitles;
    
    }
}