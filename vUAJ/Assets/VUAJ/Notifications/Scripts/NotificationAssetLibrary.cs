using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Esta clase actúa como una biblioteca centralizada de recursos para notificaciones, incluyendo
/// íconos (Sprites) y sonidos (AudioClips). Los recursos se cargan automáticamente desde las carpetas 
/// "Resources/NotificationIcons" y "Resources/NotificationSounds", y pueden ser consultados por nombre 
/// desde cualquier parte del juego.
/// </summary>
/// 
public class NotificationAssetLibrary : MonoBehaviour
{
    // Diccionarios privados para almacenar íconos y sonidos accesibles por nombre
    private Dictionary<string, Sprite> iconDict = new Dictionary<string, Sprite>();
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

    #region Singleton
    private static NotificationAssetLibrary _instance = null;

    public static NotificationAssetLibrary Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Accesibility Manager not present in scene");
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
            LoadAssets();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    /// <summary>
    /// Carga todos los íconos y sonidos de notificación desde las carpetas Resources.
    /// Los archivos deben estar en "Resources/NotificationIcons" y "Resources/NotificationSounds".
    /// </summary>
    private void LoadAssets()
    {
        // Carga todos los sprites desde la carpeta correspondiente
        Sprite[] icons = Resources.LoadAll<Sprite>("NotificationIcons");
        foreach (var icon in icons)
        {
            string key = icon.name.ToLower();
            if (!iconDict.ContainsKey(key))
                iconDict.Add(key, icon);
        }

        // Carga todos los sonidos desde la carpeta correspondiente
        AudioClip[] sounds = Resources.LoadAll<AudioClip>("NotificationSounds");
        foreach (var sound in sounds)
        {
            string key = sound.name.ToLower();
            if (!soundDict.ContainsKey(key))
                soundDict.Add(key, sound);
        }
    }

    /// <summary>
    /// Devuelve un ícono  dado su nombre. Si no se encuentra, devuelve null y muestra una advertencia.
    /// </summary>
    public Sprite GetIcon(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        name = name.ToLower();
        if (iconDict.TryGetValue(name, out var icon))
            return icon;

        Debug.LogWarning($"Icon '{name}' not found.");
        return null;
    }

    /// <summary>
    /// Devuelve un sonido dado su nombre. Si no se encuentra, devuelve null y muestra una advertencia.
    /// </summary>
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
