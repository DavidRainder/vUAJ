using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public enum NotificationPosition { TopCenter, TopLeft, TopRight, BottomLeft, BottomRight }
public enum NotificationSize { Small, Medium, Large }
public enum NotificationDuration { Seconds3, Seconds5, Seconds7, Seconds10, Seconds15, Seconds20, Unlimited }
public enum NotificationStyle { Normal, Blur, Black, Glow }
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
public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    public NotificationPosition CurrentPosition = NotificationPosition.TopLeft;
    public NotificationSize CurrentSize = NotificationSize.Medium;
    public NotificationDuration CurrentDuration = NotificationDuration.Seconds5;
    public NotificationStyle CurrentStyle = NotificationStyle.Normal;
    public NotificationLevel CurrentLevel = NotificationLevel.Standard;
    public HapticFeedbackType CurrentHapticFeedback = HapticFeedbackType.Light;
    

    public GameObject notificationPrefab;
    public Transform notificationContainer;

    public UnityEngine.UI.Image blackOverlay;
    public UnityEngine.UI.Image transparencyOverlay;

    private Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
    private bool isProcessing = false; 


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetPosition(NotificationPosition newPosition) => CurrentPosition = CurrentPosition == newPosition ? CurrentPosition : newPosition;
    public void SetSize(NotificationSize newSize) => CurrentSize = CurrentSize == newSize ? CurrentSize :  newSize;
    public void SetDuration(NotificationDuration newDuration) => CurrentDuration = CurrentDuration == newDuration ? CurrentDuration : newDuration;
    public void SetStyle(NotificationStyle newStyle) => CurrentStyle = CurrentStyle == newStyle ? CurrentStyle : newStyle;
    public void SetType(NotificationLevel newType) => CurrentLevel = CurrentLevel == newType ? CurrentLevel : newType;
    public void SetHapticFeedback(HapticFeedbackType newType) => CurrentHapticFeedback = CurrentHapticFeedback == newType ? CurrentHapticFeedback : newType;
    public void SetCanvasPreferences(Transform container, UnityEngine.UI.Image black, UnityEngine.UI.Image transparency)
    {
        notificationContainer = container;
        blackOverlay = black;
        transparencyOverlay = transparency;
    }

    public void NotifyChange()
    {
        Debug.Log($"[NotificationManager] Settings changed:\n" +
                  $"- Position: {CurrentPosition}\n" +
                  $"- Size: {CurrentSize}\n" +
                  $"- Duration: {CurrentDuration}\n" +
                  $"- Style: {CurrentStyle}\n" +
                  $"- Level: {CurrentLevel}");
    }

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

        // Start processing if not already processing
        if (!isProcessing)
        {
            StartCoroutine(ProcessNotifications());
        }
    }
    private IEnumerator ProcessNotifications()
    {
        isProcessing = true;

        while (notificationQueue.Count > 0)
        {
            var notificationData = notificationQueue.Dequeue();

            var icon = NotificationAssetLibrary.Instance.GetIcon(notificationData.iconName);
            var sound = NotificationAssetLibrary.Instance.GetSound(notificationData.soundName);

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

            float duration = ((float)notificationData.duration / 1000f) + 3f; 
            yield return new WaitForSeconds(duration);
        }

        isProcessing = false;
    }

    public void SpawnNotification(string message, string iconName, Color color, string soundName,
        NotificationPosition pos, NotificationSize size, NotificationDuration duration, HapticFeedbackType haptic, NotificationStyle type)
    {

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
