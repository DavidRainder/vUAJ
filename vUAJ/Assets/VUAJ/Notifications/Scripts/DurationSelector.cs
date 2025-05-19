using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Esta clase gestiona una serie de botones que permiten al usuario seleccionar la duraci�n
/// de una notificaci�n. Cada bot�n representa una duraci�n distinta. Cuando se selecciona un bot�n,
/// se actualiza al manager de la elecci�n y se actualiza visualmente el bot�n seleccionado.
/// </summary>
/// 
public class DurationSelector : MonoBehaviour
{
    [System.Serializable]
    public struct DurationButton
    {
        public NotificationDuration duration;  // Valor de duraci�n asociado al bot�n
        public Button button; // Referencia al bot�n UI
    }

    public DurationButton[] durationButtons; // Arreglo de botones con su duraci�n asociada

    private NotificationDuration currentSelected; // Duraci�n actualmente seleccionada

    void Start()
    {
        // Asigna a cada bot�n su correspondiente callback de selecci�n
        foreach (var db in durationButtons)
        {
            var d = db.duration;
            var btn = db.button;

            // Al hacer clic, se llama a OnDurationSelected con la duraci�n asociada
            btn.onClick.AddListener(() => OnDurationSelected(d));
        }

        // Obtiene la duraci�n actual del NotificationManager
        currentSelected = NotificationManager.Instance.CurrentDuration;

        // Actualiza visualmente los botones para reflejar el estado actual
        UpdateButtonStates();
    }

    /// <summary>
    /// Maneja la selecci�n de una duraci�n por parte del usuario.
    /// </summary>
    private void OnDurationSelected(NotificationDuration selectedDuration)
    {
        // Si se selecciona la misma duraci�n que ya est� activa, no se hace nada
        if (currentSelected == selectedDuration) return;

        // Actualiza la duraci�n seleccionada
        currentSelected = selectedDuration;

        // Informa al NotificationManager del nuevo valor
        NotificationManager.Instance.SetDuration(selectedDuration);

        // Actualiza el color de los botones para reflejar la selecci�n
        UpdateButtonStates();
    }

    /// <summary>
    /// Cambia visualmente los botones seg�n si est�n seleccionados o no.
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
