using UnityEngine;
using System.Collections.Generic;

public class NotificationAssetLibrary : MonoBehaviour
{
    public static NotificationAssetLibrary Instance { get; private set; }

    private Dictionary<string, Sprite> iconDict = new Dictionary<string, Sprite>();
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAssets();
    }

    private void LoadAssets()
    {
        Sprite[] icons = Resources.LoadAll<Sprite>("NotificationIcons");
        foreach (var icon in icons)
        {
            string key = icon.name.ToLower();
            if (!iconDict.ContainsKey(key))
                iconDict.Add(key, icon);
        }

        AudioClip[] sounds = Resources.LoadAll<AudioClip>("NotificationSounds");
        foreach (var sound in sounds)
        {
            string key = sound.name.ToLower();
            if (!soundDict.ContainsKey(key))
                soundDict.Add(key, sound);
        }
    }

    public Sprite GetIcon(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        name = name.ToLower();
        if (iconDict.TryGetValue(name, out var icon))
            return icon;

        Debug.LogWarning($"Icon '{name}' not found.");
        return null;
    }

    public AudioClip GetSound(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        name = name.ToLower();
        if (soundDict.TryGetValue(name, out var sound))
            return sound;

        Debug.LogWarning($"Sound '{name}' not found.");
        return null;
    }
}
