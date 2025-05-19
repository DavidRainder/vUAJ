using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DurationSelector : MonoBehaviour
{
    [System.Serializable]
    public struct DurationButton
    {
        public NotificationDuration duration;
        public Button button;
    }

    public DurationButton[] durationButtons;

    private NotificationDuration currentSelected;

    void Start()
    {
        foreach (var db in durationButtons)
        {
            var d = db.duration;
            var btn = db.button;

            btn.onClick.AddListener(() => OnDurationSelected(d));
        }

        currentSelected = NotificationManager.Instance.CurrentDuration;
        UpdateButtonStates();
    }

    private void OnDurationSelected(NotificationDuration selectedDuration)
    {
        if (currentSelected == selectedDuration) return;

        currentSelected = selectedDuration;

        NotificationManager.Instance.SetDuration(selectedDuration);

        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        foreach (var db in durationButtons)
        {
            bool isSelected = db.duration == currentSelected;

            var colors = db.button.colors;
            colors.normalColor = isSelected ? new Color32(0x54, 0x80, 0xFF, 0xFF) : Color.white;
            db.button.colors = colors;
        }
    }
}
