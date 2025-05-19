using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Esta clase vincula un dropdown UI  con el sistema de notificaciones.
/// Permite al usuario seleccionar entre distintos niveles de notificación (por ejemplo, estándar o asistido),
/// y actualiza automáticamente el tipo en el NotificationManager según la opción elegida.
/// Además, gestiona correctamente los listeners para evitar referencias colgantes al desactivar el objeto.
/// </summary>
public class LevelDropdown : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown levelDropdown;


    private void Start()
    {
        // Si no se asignó manualmente en el inspector, lo busca en el mismo GameObject
        if (levelDropdown == null)
            levelDropdown = GetComponent<TMPro.TMP_Dropdown>();

        // Se suscribe al evento de cambio de valor del dropdown
        levelDropdown.onValueChanged.AddListener(OnLevelChanged);

        // Establece el nivel actual al valor por defecto del dropdown
        levelDropdown.value = (int)NotificationManager.Instance.CurrentLevel;
        SetLevelFromDropdown(levelDropdown.value);
    }

    /// <summary>
    /// Llamado automáticamente cuando el usuario cambia la opción del dropdown.
    /// </summary>
    private void OnLevelChanged(int index)
    {
        SetLevelFromDropdown(levelDropdown.value);
    }

    /// <summary>
    /// Cambia el tipo de notificación en el NotificationManager según el índice del dropdown.
    /// </summary>
    private void SetLevelFromDropdown(int index)
    {
        // Si el índice es 0, se selecciona Standard; si no, Assisted.
        NotificationLevel level = index == 0 ? NotificationLevel.Standard : NotificationLevel.Assisted;

        // Establece el tipo en el NotificationManager
        NotificationManager.Instance?.SetType(level);
    }

    /// <summary>
    /// Elimina los listeners al desactivar el objeto, para evitar referencias persistentes no deseadas.
    /// </summary>
    void OnDisable()
    {
        if (levelDropdown != null)
            levelDropdown.onValueChanged.RemoveAllListeners();
    }
}
