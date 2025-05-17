using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Añadir a un objeto de la escena para que lea los subtitulos que se le pasen
public class TTSSubtitles : MonoBehaviour
{
    [SerializeField]
    public KeyCode key_repeat = KeyCode.R; //Repetir lo ya dicho

    [SerializeField]
    string nameFile; //Debe de guardarse en Application.persistentDataPath

    List<string> subtitles = new List<string>();

    int _nextSubtitle = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        subtitles = JSONParser.FromJson(nameFile);
        subtitles.Add(""); //Añade una cadena vacia al final para que no se repita la ultima linea de texto siempre
    }

    //Este metodo lee los subtitulos linea a linea
    public void ReadNextSubtitle()
    {
        TTSManager.m_Instance.StartSpeech(subtitles[_nextSubtitle]);
        if(_nextSubtitle+1 < subtitles.Count)
            _nextSubtitle++;
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
