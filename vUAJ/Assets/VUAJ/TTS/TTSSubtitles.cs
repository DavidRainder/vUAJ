using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Añadir a un objeto de la escena para que lea los subtitulos que se le pasen
public class TTSSubtitles : MonoBehaviour
{
    #region Singleton
    private static TTSSubtitles _instance = null;

    public static TTSSubtitles Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("TTS Subtitles not present in scene");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField]
    string nameFile; //Debe de guardarse en Application.persistentDataPath


    List<string> subtitles;

    int _nextSubtitle = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        subtitles = JSONParser.FromJson(nameFile);
        if(subtitles == null ) { subtitles = new List<string>(); }
        subtitles.Add(""); //Añade una cadena vacia al final para que no se repita la ultima linea de texto siempre
    }

    //Este metodo lee los subtitulos linea a linea
    public void ReadNextSubtitle()
    {
        if (TTSManager.Instance.GetTTSactive())
        {
            TTSManager.Instance.StartSpeech(subtitles[_nextSubtitle]);
            if (_nextSubtitle + 1 < subtitles.Count)
                _nextSubtitle++;

        }
    }

    //Cambia el file de los subtitulos durante ejecucion (por ejemplo para hablar con otro NPC, etc)
    public void ChangeSubtitles(string newNameFile)
    {
        nameFile = newNameFile;
        subtitles = JSONParser.FromJson(nameFile);
        _nextSubtitle = 0;
        subtitles.Add("");
    }
}
