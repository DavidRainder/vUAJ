using System;
using TMPro;
using UnityEngine;

public class VolumePerceptionManager : MonoBehaviour
{
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

    [SerializeField]
    public Color volumeColor = new Color(1, 1, 1, 0.6f);

    public event Action<bool> changeShowColliders;
    public bool showColliders = false;

    public void setShowColliders(bool show)
    {
        showColliders = show;
        if(changeShowColliders != null)
            changeShowColliders(showColliders);
    }
}
