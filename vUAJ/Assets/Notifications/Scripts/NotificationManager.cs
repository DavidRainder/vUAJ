using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

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
        var icon = NotificationAssetLibrary.Instance.GetIcon(iconName);
        var sound = NotificationAssetLibrary.Instance.GetSound(soundName);

        var notifGO = Instantiate(notificationPrefab, notificationContainer);
        var ui = notifGO.GetComponent<NotificationUI>();

        if (ui != null)
        {
            ui.SetUp(
                message,
                icon,
                color,
                sound,
                haptic,
                CurrentPosition,
                CurrentSize,
                CurrentStyle,
                CurrentDuration
            );
        }
        NotifyChange();
    }

    public void SpawnNotification(string message, string iconName, Color color, string soundName,
        NotificationPosition pos, NotificationSize size, NotificationDuration duration, HapticFeedbackType haptic, NotificationStyle type)
    {
        var icon = NotificationAssetLibrary.Instance.GetIcon(iconName);
        var sound = NotificationAssetLibrary.Instance.GetSound(soundName);

        var notifGO = Instantiate(notificationPrefab, notificationContainer);
        var ui = notifGO.GetComponent<NotificationUI>();

        SetPosition(pos);
        SetSize(size);
        SetDuration(duration);
        if (ui != null)
        {
            ui.SetUp(
                message,
                icon,
                color,
                sound,
                haptic,
                pos,
                size,
                type,
                duration
            );
        }
        NotifyChange();
    }

}
