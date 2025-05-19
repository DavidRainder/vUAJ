using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

/// <summary>
/// NotificationManager es el componente central encargado de gestionar y mostrar notificaciones en el juego.
/// Administra una cola de notificaciones, aplicando configuraciones visuales (posición, tamaño, estilo),
/// temporales (duración), sonoras (AudioClip) y hápticas (feedback táctil).
/// 
/// Utiliza un patrón Singleton para asegurar que solo exista una instancia persistente en el juego.
/// Las notificaciones se basan en datos configurables (NotificationData) y se instancian usando un prefab UI.
/// 
/// Las referencias a elementos como el contenedor de notificaciones (Canvas) y overlays deben establecerse
/// en la escena activa, ya que no pueden ser persistidas fuera de ella.
/// </summary>
/// 
public enum NotificationPosition { TopCenter, TopLeft, TopRight, BottomLeft, BottomRight }
public enum NotificationSize { Small, Medium, Large }
public enum NotificationDuration { Seconds3, Seconds5, Seconds7, Seconds10, Seconds15, Seconds20, Unlimited }
public enum NotificationStyle { Normal, Translucent, Black, Glow }
public enum NotificationLevel { Standard, Assisted}
public enum HapticFeedbackType
{
    None,
    Light,
    Medium,
    Heavy,
    Pulse
}
public struct NotificationData
{
    public string message;
    public string iconName;
    public Color color;
    public string soundName;
    public HapticFeedbackType haptic;
    public NotificationPosition position;
    public NotificationSize size;
    public NotificationStyle style;
    public NotificationDuration duration;
}

public struct NotificationInfoSerialized
{
    public NotificationPosition CurrentPosition;
    public NotificationSize CurrentSize;
    public NotificationDuration CurrentDuration;
    public NotificationStyle CurrentStyle;
    public NotificationLevel CurrentLevel;
}

[RequireComponent(typeof(Serializer))]
public class NotificationManager : MonoBehaviour
{
    // Configuraciones default de notificación
    public NotificationPosition CurrentPosition = NotificationPosition.TopLeft;
    public NotificationSize CurrentSize = NotificationSize.Medium;
    public NotificationDuration CurrentDuration = NotificationDuration.Seconds5;
    public NotificationStyle CurrentStyle = NotificationStyle.Normal;
    public NotificationLevel CurrentLevel = NotificationLevel.Standard;
    public HapticFeedbackType CurrentHapticFeedback = HapticFeedbackType.Light;

    // Prefab de notificación y su contenedor en la escena
    public GameObject notificationPrefab;
    public Transform notificationContainer;

    // Capas visuales opcionales
    public UnityEngine.UI.Image blackOverlay;
    public UnityEngine.UI.Image transparencyOverlay;

    // Cola interna de notificaciones y control de procesamiento
    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private bool isProcessing = false;


    #region Singleton
    private static NotificationManager _instance = null;

    public static NotificationManager Instance
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

    // Métodos para actualizar configuraciones si cambian
    public void SetPosition(NotificationPosition newPosition) { 
        CurrentPosition = CurrentPosition == newPosition ? CurrentPosition : newPosition;
        SaveData();
    }
    public void SetSize(NotificationSize newSize) { 
        CurrentSize = CurrentSize == newSize ? CurrentSize :  newSize;
        SaveData();
    }
    public void SetDuration(NotificationDuration newDuration) { 
        CurrentDuration = CurrentDuration == newDuration ? CurrentDuration : newDuration;
        SaveData();
    }
    public void SetStyle(NotificationStyle newStyle) {
        CurrentStyle = CurrentStyle == newStyle ? CurrentStyle : newStyle;
        SaveData();
    }
    public void SetType(NotificationLevel newType) { 
        CurrentLevel = CurrentLevel == newType ? CurrentLevel : newType;
        SaveData();
    }

    public void SetHapticFeedback(HapticFeedbackType newType) {
        CurrentHapticFeedback = CurrentHapticFeedback == newType ? CurrentHapticFeedback : newType;
        SaveData();
    }

    NotificationInfoSerialized serializedInfo = new NotificationInfoSerialized();

    void SaveData()
    {
        if(serializer)
        {
            serializer.Clear();

            serializedInfo.CurrentDuration = CurrentDuration;
            serializedInfo.CurrentPosition = CurrentPosition;
            serializedInfo.CurrentSize = CurrentSize;
            serializedInfo.CurrentStyle = CurrentStyle;
            serializedInfo.CurrentLevel = CurrentLevel;

            serializer.Serialize(serializedInfo);
            serializer.WriteToJSON("NotificationSettings/", gameObject.name);
        }
    }

    Serializer serializer;
    private void Start()
    {
        serializer = GetComponent<Serializer>();
        if(serializer && serializer.getFromJSONStruct("NotificationSettings/"+gameObject.name, out NotificationInfoSerialized info) != -1)
        {
            serializedInfo = info;

            CurrentPosition = info.CurrentPosition;
            CurrentSize = info.CurrentSize;
            CurrentDuration = info.CurrentDuration;
            CurrentStyle = info.CurrentStyle;
            CurrentLevel = info.CurrentLevel;
        }
    }

    /// <summary>
    /// Establece las referencias al canvas y overlays. Debe llamarse desde la escena que contiene estos elementos.
    /// </summary>
    public void SetCanvasPreferences(Transform container, UnityEngine.UI.Image black, UnityEngine.UI.Image transparency)
    {
        notificationContainer = container;
        blackOverlay = black;
        transparencyOverlay = transparency;
    }

    /// <summary>
    /// Imprime en consola los valores actuales de configuración de las notificaciones.
    /// </summary>
    public void NotifyChange()
    {
        Debug.Log($"[NotificationManager] Settings changed:\n" +
                  $"- Position: {CurrentPosition}\n" +
                  $"- Size: {CurrentSize}\n" +
                  $"- Duration: {CurrentDuration}\n" +
                  $"- Style: {CurrentStyle}\n" +
                  $"- Level: {CurrentLevel}");
    }

    /// <summary>
    /// Crea y encola una nueva notificación con la configuración actual del sistema.
    /// </summary>
    public void SpawnNotification(string message, string iconName, Color color, string soundName, HapticFeedbackType haptic)
    {
        var notificationData = new NotificationData
        {
            message = message,
            iconName = iconName,
            color = color,
            soundName = soundName,
            haptic = haptic,
            position = CurrentPosition,
            size = CurrentSize,
            style = CurrentStyle,
            duration = CurrentDuration
        };

        notificationQueue.Enqueue(notificationData);

        // Inicia el procesamiento si no está activo
        if (!isProcessing)
        {
            StartCoroutine(ProcessNotifications());
        }
    }

    /// <summary>
    /// Procesa la cola de notificaciones de forma secuencial.
    /// </summary>
    private IEnumerator ProcessNotifications()
    {
        isProcessing = true;

        while (notificationQueue.Count > 0)
        {
            var notificationData = notificationQueue.Dequeue();

            // Carga los recursos visuales y sonoros desde la biblioteca de assets
            var icon = NotificationAssetLibrary.Instance.GetIcon(notificationData.iconName);
            var sound = NotificationAssetLibrary.Instance.GetSound(notificationData.soundName);

            // Instancia la notificación UI y la configura
            var notifGO = Instantiate(notificationPrefab, notificationContainer);
            var ui = notifGO.GetComponent<NotificationUI>();

            if (ui != null)
            {
                ui.SetUp(
                    notificationData.message,
                    icon,
                    notificationData.color,
                    sound,
                    notificationData.haptic,
                    notificationData.position,
                    notificationData.size,
                    notificationData.style,
                    notificationData.duration
                );
            }

            NotifyChange();

            // Tiempo de espera aproximado para mostrar la notificación
            float duration = ((float)notificationData.duration / 1000f) + 3f; 
            yield return new WaitForSeconds(duration);
        }

        isProcessing = false;
    }

    /// <summary>
    /// Variante sobrecargada de SpawnNotification que permite definir una configuración personalizada para una notificación específica.
    /// </summary>
    public void SpawnNotification(string message, string iconName, Color color, string soundName,
        NotificationPosition pos, NotificationSize size, NotificationDuration duration, HapticFeedbackType haptic, NotificationStyle type)
    {
        // Aplica la configuración personalizada
        SetPosition(pos);
        SetSize(size);
        SetDuration(duration);

        var notificationData = new NotificationData
        {
            message = message,
            iconName = iconName,
            color = color,
            soundName = soundName,
            haptic = haptic,
            position = pos,
            size = size,
            style = type,
            duration = duration
        };

        notificationQueue.Enqueue(notificationData);

        if (!isProcessing)
        {
            StartCoroutine(ProcessNotifications());
        }
    }

}
