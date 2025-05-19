using System;
using UnityEngine;
using UnityEngine.UI;

public class AccessibilityPanelController : MonoBehaviour
{
    public GameObject[] panels;
    public Button[] buttons;

    private int currentIndex = -1;

    private void Start()
    {
        if (panels.Length > 0)
            ShowPanel(0);
    }

    public void ShowPanel(int index)
    {
        if (index == currentIndex) return;

        for (int i = 0; i < panels.Length; i++)
        {
            bool active = (i == index);
            panels[i].SetActive(active);
            buttons[i].interactable = !active;

            var colors = buttons[i].colors;
            colors.normalColor = active ? new Color32(0x54, 0x80, 0xFF, 0xFF) : Color.white;
            buttons[i].colors = colors;
        }

        currentIndex = index;
    }
}
