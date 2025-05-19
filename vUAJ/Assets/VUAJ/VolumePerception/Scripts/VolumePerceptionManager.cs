using System;
using UnityEngine;

// Manager que controla la opcion de accesibilidad de Volume Perception
public class VolumePerceptionManager : MonoBehaviour
{
    // Codigo de Singleton
    #region Singleton
    private static VolumePerceptionManager _instance = null;

    public static VolumePerceptionManager Instance
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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Color del que se quiere que sean las mallas que muestran los colliders
    [SerializeField]
    public Color volumeColor = new Color(1, 1, 1, 0.6f);

    // Action que se lanza al activar o desactivar la opcion de volume perception
    public event Action<bool> changeShowColliders;

    // Booleano que indica si se muestran los colliders o no
    public bool showColliders = false;

    // Activa o desactiva que se muestren los colliders o no
    public void setShowColliders(bool show)
    {
        showColliders = show;
        if(changeShowColliders != null)
            changeShowColliders(showColliders);
    }
}
