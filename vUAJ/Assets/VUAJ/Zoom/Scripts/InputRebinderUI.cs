using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using System;

public class InputRebinderUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public int bindingIndex;
    public TextMeshProUGUI bindingDisplayText;
    public Button rebindButton;

    // Acción -> bindingIndex -> KeyCode
    public static Dictionary<string, Dictionary<int, KeyCode>> keyMap = new();
    public static HashSet<KeyCode> usedKeys = new();

    private void Start()
    {
        RegisterInitialKey();
        UpdateBindingDisplay();
        rebindButton.onClick.AddListener(() => StartRebinding());
    }

    void UpdateBindingDisplay()
    {
        string actionName = actionReference.action.name;

        if (keyMap.TryGetValue(actionName, out var bindings) &&
            bindings.TryGetValue(bindingIndex, out var key))
        {
            bindingDisplayText.text = key.ToString();
        }
        else
        {
            bindingDisplayText.text = "Unassigned";
        }
    }

    private void RegisterInitialKey()
    {
        var action = actionReference.action;
        string actionName = action.name;

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            Debug.LogError($"Invalid binding index in {actionName}");
            return;
        }

        var binding = action.bindings[bindingIndex];
        var control = InputSystem.FindControl(binding.effectivePath);
        KeyCode key = ConvertControlToKeyCode(control);

        if (!keyMap.ContainsKey(actionName))
            keyMap[actionName] = new Dictionary<int, KeyCode>();

        keyMap[actionName][bindingIndex] = key;

        usedKeys.Add(key);
    }

    public void StartRebinding()
    {
        rebindButton.interactable = false;
        bindingDisplayText.text = "...";

        var action = actionReference.action;
        string actionName = action.name;

        if (!keyMap.ContainsKey(actionName))
            keyMap[actionName] = new();

        var previousKey = keyMap[actionName].ContainsKey(bindingIndex)
            ? keyMap[actionName][bindingIndex]
            : KeyCode.None;

        action.actionMap.Disable();

        var rebindOp = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(op =>
            {
                var control = op.selectedControl;
                var newKey = ConvertControlToKeyCode(control);

                op.Dispose();

                // SI la nueva tecla ya está en uso por otra acción
                if (usedKeys.Contains(newKey) && newKey != previousKey)
                {
                    Debug.LogWarning($"Key '{newKey}' is already in use.");
                    bindingDisplayText.text = $"Key ({newKey}) in use. Please choose another.";
                    rebindButton.interactable = true;

                    // Revertir visual y lógica
                    action.ApplyBindingOverride(bindingIndex, action.bindings[bindingIndex].effectivePath);
                    action.actionMap.Enable();
                    return;
                }

                // ✅ Actualización segura del mapa
                if (previousKey != KeyCode.None && newKey != previousKey)
                    usedKeys.Remove(previousKey);

                usedKeys.Add(newKey);
                keyMap[actionName][bindingIndex] = newKey;

                UpdateBindingDisplay();
                rebindButton.interactable = true;
                action.actionMap.Enable();
            })
            .OnCancel(op =>
            {
                op.Dispose();
                UpdateBindingDisplay();
                rebindButton.interactable = true;
                action.actionMap.Enable();
            })
            .Start();
    }

    private KeyCode ConvertControlToKeyCode(InputControl control)
    {
        if (control is KeyControl keyControl)
        {
            try
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), keyControl.name, true);
            }
            catch
            {
                Debug.LogWarning($"Unknown key: {keyControl.name}");
            }
        }

        return KeyCode.None;
    }
}
