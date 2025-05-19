using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Esta clase gestiona una serie de botones que permiten al usuario seleccionar la duración
/// de una notificación. Cada botón representa una duración distinta. Cuando se selecciona un botón,
/// se actualiza al manager de la elección y se actualiza visualmente el botón seleccionado.
/// </summary>
/// 
public class DurationSelector : MonoBehaviour
{
    [System.Serializable]
    public struct DurationButton
    {
        public NotificationDuration duration;  // Valor de duración asociado al botón
        public Button button; // Referencia al botón UI
    }

    public DurationButton[] durationButtons; // Arreglo de botones con su duración asociada

    private NotificationDuration currentSelected; // Duración actualmente seleccionada

    void Start()
    {
        // Asigna a cada botón su correspondiente callback de selección
        foreach (var db in durationButtons)
        {
            var d = db.duration;
            var btn = db.button;

            // Al hacer clic, se llama a OnDurationSelected con la duración asociada
            btn.onClick.AddListener(() => OnDurationSelected(d));
        }

        // Obtiene la duración actual del NotificationManager
        currentSelected = NotificationManager.Instance.CurrentDuration;

        // Actualiza visualmente los botones para reflejar el estado actual
        UpdateButtonStates();
    }

    /// <summary>
    /// Maneja la selección de una duración por parte del usuario.
    /// </summary>
    private void OnDurationSelected(NotificationDuration selectedDuration)
    {
        // Si se selecciona la misma duración que ya está activa, no se hace nada
        if (currentSelected == selectedDuration) return;

        // Actualiza la duración seleccionada
        currentSelected = selectedDuration;

        // Informa al NotificationManager del nuevo valor
        NotificationManager.Instance.SetDuration(selectedDuration);

        // Actualiza el color de los botones para reflejar la selección
        UpdateButtonStates();
    }

    /// <summary>
    /// Cambia visualmente los botones según si están seleccionados o no.
    /// </summary>
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
