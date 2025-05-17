using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class TTSSubtitles : MonoBehaviour
{
    [SerializeField]
    public KeyCode key_repeat = KeyCode.R; //Repetir lo ya dicho

    [SerializeField]
    string nameFile; // Must be saved in application.datapath

    List<string> subtitles = new List<string>();

    int _nextSubtitle = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        subtitles = JSONParser.FromJson(nameFile);
        subtitles.Add("");
    }

    //Call this method to read subtitles
    public void ReadNextSubtitle()
    {
        TTSManager.m_Instance.StartSpeech(subtitles[_nextSubtitle]);
        if(_nextSubtitle+1 < subtitles.Count)
            _nextSubtitle++;
    }

    //Change subtitles file during game
    public void ChangeSubtitles(string newNameFile)
    {
        nameFile = newNameFile;
        subtitles = JSONParser.FromJson(nameFile);
        _nextSubtitle = 0;
        subtitles.Add("");
    }
}
