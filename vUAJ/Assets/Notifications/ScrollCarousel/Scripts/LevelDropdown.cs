using UnityEngine;
using UnityEngine.UI;

public class LevelDropdown : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Dropdown levelDropdown;


    private void Start()
    {
        if (levelDropdown == null)
            levelDropdown = GetComponent<TMPro.TMP_Dropdown>();

        levelDropdown.onValueChanged.AddListener(OnLevelChanged);

        SetLevelFromDropdown(levelDropdown.value);
    }

    private void OnLevelChanged(int index)
    {
        SetLevelFromDropdown(index);
    }

    private void SetLevelFromDropdown(int index)
    {
        NotificationLevel level = index == 0 ? NotificationLevel.Standard : NotificationLevel.Assisted;
        NotificationManager.Instance?.SetType(level);
    }

    void OnDisable()
    {
        if (levelDropdown != null)
            levelDropdown.onValueChanged.RemoveAllListeners();
    }
}
