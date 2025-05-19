using Platformer;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCSubs : MonoBehaviour
{

    bool _playerInTrigger = false;

    [SerializeField]
    TextMeshProUGUI SubtitlesText;

    int _nextSubtitle = 0;

    [SerializeField]
    string nameFile; //Debe de guardarse en Application.persistentDataPath


    List<string> subtitles;

    [SerializeField]
    public KeyCode key_continue = KeyCode.Space;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        subtitles = JSONParser.FromJson(nameFile);
        if (subtitles == null) { subtitles = new List<string>(); }
        subtitles.Add("");
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerInTrigger && Input.GetKeyDown(key_continue))
        {
            Debug.Log("Le has dado a N");
            SubtitlesText.text = subtitles[_nextSubtitle];
            TTSSubtitles.Instance.ReadNextSubtitle();
            if (_nextSubtitle + 1 < subtitles.Count)
                _nextSubtitle++;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Entra");
            _playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            Debug.Log("Sale");
            _playerInTrigger = false;
        }
    }
}
