using System;
using UnityEngine;

/// <summary>
/// Esta clase se encarga de ajustar automáticamente un Canvas en modo World Space para que se posicione
/// frente a una cámara específica y se escale en función de una resolución de referencia. Esto es 
/// necesario para cualquier canvas ya que la herramienta de Zoom lo requiere. Permite al desarrolador
/// trabajar con un canvas en modo Overlay facilmente sin preocuparse.
/// </summary>


[RequireComponent(typeof(Canvas))]  // Obligatorio que el GameObject sea un Canvas
public class AutoScaleCanvasToCamera : MonoBehaviour
{
    public Camera targetCamera; // Cámara objetivo frente a la cual se posicionará el canvas
    public float distanceFromCamera = 2f; // Distancia a la que se colocará el canvas respecto a la cámara
    public Vector2 referenceResolution = new Vector2(1920, 1080); // Resolución de referencia para el escalado del canvas

    private Canvas canvas; // Referencia a su componente Canvas

    void Start()
    {
        canvas = GetComponent<Canvas>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main; // Si no se asignó una cámara, se usa la principal
        }

        // Configura el modo de renderizado como World Space
        canvas.renderMode = RenderMode.WorldSpace;

        if (canvas.worldCamera == null)
            canvas.worldCamera = targetCamera; // Asigna la cámara objetivo si no se ha definido una cámara mundial

        // Llama al método que posiciona y escala el canvas
        PositionAndScaleCanvas();
    }

    void PositionAndScaleCanvas()
    {
        // Pone el canvas delante de la cámara
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
        transform.rotation = targetCamera.transform.rotation;

        // Calcula la altura visible en unidades del mundo dependiendo del tipo de cámara
        float screenHeightUnits;
        if (targetCamera.orthographic)
        {
            // Altura del área visible para cámara ortográfica
            screenHeightUnits = targetCamera.orthographicSize * 2f;
        }
        else
        {
            // Para cámara en perspectiva, se usa trigonometría para calcular la altura visible
            float fovInRadians = targetCamera.fieldOfView * Mathf.Deg2Rad;
            screenHeightUnits = 2f * distanceFromCamera * Mathf.Tan(fovInRadians / 2f);
        }

        // Calcula cuántas unidades del mundo equivale un píxel
        float unitsPerPixel = screenHeightUnits / referenceResolution.y;

        // Calcula el tamaño del canvas en unidades del mundo
        float canvasWidth = referenceResolution.x * unitsPerPixel;
        float canvasHeight = referenceResolution.y * unitsPerPixel;

        RectTransform rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta = referenceResolution; // Establece el tamaño del rectángulo en píxeles de referencia
        rt.localScale = Vector3.one * unitsPerPixel; // Escala el canvas para ajustarse al tamaño en unidades del mundo
    }
}