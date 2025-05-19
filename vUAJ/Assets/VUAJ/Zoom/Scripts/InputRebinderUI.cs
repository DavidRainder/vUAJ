using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using System;

/// <summary>
/// La clase `InputRebinderUI` permite gestionar el mapeo de teclas para el Zoom. 
/// La clase maneja tanto el registro de las teclas iniciales como la lógica de reasignación a través de un botón interactivo. 
/// El sistema asegura que no haya conflictos con teclas ya utilizadas, mantienendo un mapa de las teclas asignadas a cada acción, 
/// y avisando al jugador si no ha podido cambair la letra correctamente.
/// </summary>
public class InputRebinderUI : MonoBehaviour
{
    public InputActionReference actionReference; // Referencia a la acción de entrada en el sistema de Input
    public int bindingIndex; // Índice del binding dentro de la acción
    public TextMeshProUGUI bindingDisplayText; // Texto de UI que muestra la tecla asignada
    public Button rebindButton; // Botón que permite iniciar el proceso de reasignación

    // Mapa que guarda las teclas asignadas a cada acción y su índice de binding
    public static Dictionary<string, Dictionary<int, KeyCode>> keyMap = new();
    // Conjunto de teclas que ya están en uso para evitar duplicados
    public static HashSet<KeyCode> usedKeys = new();

    Serializer serializer = null;

    [System.Serializable]
    struct InputActionBindingPathSerializedInfo
    {
        public string effectivePath;
    }

    private void Start()
    {
        // Inicializa la asignación de la tecla y configura la UI
        serializer = GetComponent<Serializer>();

        if(serializer && 
            serializer.getFromJSONStruct(
                "InputActions/" + actionReference.action.name + "_" + bindingIndex, 
            out InputActionBindingPathSerializedInfo action) != -1) 
        {
            actionReference.action.ApplyBindingOverride(bindingIndex, action.effectivePath);
        }

        RegisterInitialKey();
        UpdateBindingDisplay();
        rebindButton.onClick.AddListener(() => StartRebinding());
    }

    /// <summary>
    /// Actualiza la visualización en la interfaz de usuario (UI) para mostrar la tecla actualmente asignada a una acción específica.
    /// Si no hay tecla asignada, muestra el mensaje "Unassigned".
    /// </summary>
    void UpdateBindingDisplay()
    {
        // Obtiene el nombre de la acción asociada al InputActionReference.
        string actionName = actionReference.action.name;

        // Verifica si la acción tiene una tecla asignada en el mapa de teclas.
        if (keyMap.TryGetValue(actionName, out var bindings) &&
            bindings.TryGetValue(bindingIndex, out var key))
        {
            // Si se encuentra la tecla, actualiza el texto de la UI para mostrar la tecla asignada.
            bindingDisplayText.text = key.ToString();
        }
        else
        {
            // Si no se encuentra la tecla asignada, muestra "Unassigned" en la UI.
            bindingDisplayText.text = "Unassigned";
        }
    }

    /// <summary>
    /// Registra la tecla inicial asignada a una acción en el mapa de teclas (keyMap).
    /// Asocia la tecla con el índice correspondiente y la acción de entrada.
    /// Si el índice es inválido, se muestra un error.
    /// </summary>
    private void RegisterInitialKey()
    {
        // Obtiene la acción asociada al InputActionReference.
        var action = actionReference.action;

        // Obtiene el nombre de la acción.
        string actionName = action.name;

        // Verifica si el índice de binding es válido (dentro del rango de bindings de la acción).
        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            // Si el índice es inválido, muestra un mensaje de error y termina la función.
            Debug.LogError($"Invalid binding index in {actionName}");
            return;
        }

        // Obtiene el binding asociado al índice de binding.
        var binding = action.bindings[bindingIndex];

        // Encuentra el control de entrada correspondiente usando el path efectivo del binding.
        var control = InputSystem.FindControl(binding.effectivePath);

        // Convierte el control encontrado a un KeyCode.
        KeyCode key = ConvertControlToKeyCode(control);

        // Si no existe aún una entrada para esta acción en el mapa de teclas, crea una nueva entrada.
        if (!keyMap.ContainsKey(actionName))
            keyMap[actionName] = new Dictionary<int, KeyCode>();

        // Asocia el KeyCode a la acción y el índice de binding en el mapa de teclas.
        keyMap[actionName][bindingIndex] = key;

        // Añade la tecla utilizada al conjunto de teclas utilizadas para evitar duplicados.
        usedKeys.Add(key);
    }

    /// <summary>
    /// Inicia el proceso de reasignación para una tecla asociada al un índice de la acción de entrada.
    /// Desactiva el botón mientras se lleva a cabo el proceso y maneja el caso en que una tecla ya esté en uso.
    /// </summary>
    public void StartRebinding()
    {
        // Desactiva el botón de reasignación mientras se lleva a cabo el proceso.
        rebindButton.interactable = false;

        // Cambia el texto de la UI para indicar que se está esperando la nueva tecla.
        bindingDisplayText.text = "...";

        // Obtiene la acción asociada al InputActionReference.
        var action = actionReference.action;

        // Obtiene el nombre de la acción.
        string actionName = action.name;

        // Si la acción no tiene un mapa de teclas, crea uno nuevo para esa acción.
        if (!keyMap.ContainsKey(actionName))
            keyMap[actionName] = new();

        // Guarda la tecla anterior si ya está asignada.
        var previousKey = keyMap[actionName].ContainsKey(bindingIndex)
            ? keyMap[actionName][bindingIndex]
            : KeyCode.None;

        // Desactiva el mapa de acciones para permitir la reconfiguración de teclas.
        action.actionMap.Disable();

        // Inicia la reasignación interactivo y configura las acciones a realizar durante el proceso.
        var rebindOp = action.PerformInteractiveRebinding(bindingIndex)
            // Permite cancelar la reasignación presionando la tecla Escape.
            .WithCancelingThrough("<Keyboard>/escape")
            // Espera 0.1 segundos para que la reasignación no se interrumpa rápidamente.
            .OnMatchWaitForAnother(0.1f)
            // Al completar la reasignación, se ejecuta el siguiente código.
            .OnComplete(op =>
            {
                // Obtiene el control seleccionado después del reasignación.
                var control = op.selectedControl;
                // Convierte el control a un KeyCode.
                var newKey = ConvertControlToKeyCode(control);

                // Libera los recursos del reasignación.
                op.Dispose();

                // Verifica si la nueva tecla ya está siendo usada por otra acción.
                if (usedKeys.Contains(newKey) && newKey != previousKey)
                {
                    // Si la tecla ya está en uso, muestra una advertencia y permite seleccionar otra tecla.
                    Debug.LogWarning($"Key '{newKey}' is already in use.");
                    bindingDisplayText.text = $"Key ({newKey}) in use. Please choose another.";
                    rebindButton.interactable = true;

                    // Revierte la acción visualmente y lógicamente.
                    action.ApplyBindingOverride(bindingIndex, action.bindings[bindingIndex].effectivePath);
                    action.actionMap.Enable();
                    return;
                }

                // Si la tecla anterior no es la misma que la nueva, elimina la anterior del conjunto de teclas usadas.
                if (previousKey != KeyCode.None && newKey != previousKey)
                    usedKeys.Remove(previousKey);

                // Añade la nueva tecla al conjunto de teclas usadas.
                usedKeys.Add(newKey);

                // Actualiza el mapa de teclas con la nueva tecla.
                keyMap[actionName][bindingIndex] = newKey;

                // Actualiza la interfaz de usuario con la nueva tecla asignada.
                UpdateBindingDisplay();

                // Reactiva el botón de reasignación.
                rebindButton.interactable = true;

                // Vuelve a habilitar el mapa de acciones.
                action.actionMap.Enable();

                if(serializer)
                {
                    serializer.Clear();
                    InputActionBindingPathSerializedInfo info = new InputActionBindingPathSerializedInfo();
                    info.effectivePath = action.bindings[bindingIndex].effectivePath;
                    serializer.Serialize(info);
                    serializer.WriteToJSON("InputActions", action.name + "_" + bindingIndex);
                }
            })
            // Si la reasignación se cancela, se realiza lo siguiente.
            .OnCancel(op =>
            {
                // Libera los recursos del reasignación.
                op.Dispose();

                // Actualiza la interfaz de usuario y reactiva el botón de reasignación.
                UpdateBindingDisplay();
                rebindButton.interactable = true;

                // Vuelve a habilitar el mapa de acciones.
                action.actionMap.Enable();
            })
            // Comienza el proceso de reasignación.
            .Start();
    }

    /// <summary>
    /// Convierte la entrada del usuario a un valor correspondiente de KeyCode.
    /// Esta función se utiliza para traducir controles de teclado detectados a las teclas representadas por `KeyCode` en Unity.
    /// </summary>
    private KeyCode ConvertControlToKeyCode(InputControl control)
    {
        // Si el control es de tipo KeyControl (es decir, un control de teclado).
        if (control is KeyControl keyControl)
        {
            try
            {
                // Intenta convertir el nombre del control a un valor de KeyCode.
                return (KeyCode)Enum.Parse(typeof(KeyCode), keyControl.name, true);
            }
            catch
            {
                // Si la conversión falla (si el nombre del control no es válido), muestra un mensaje de advertencia.
                Debug.LogWarning($"Unknown key: {keyControl.name}");
            }
        }

        // Si no es un KeyControl, o si no se pudo convertir, devuelve `KeyCode.None`.
        return KeyCode.None;
    }
}
