using System;
using UnityEngine;

/// <summary>
/// Esta clase se encarga de ajustar autom�ticamente un Canvas en modo World Space para que se posicione
/// frente a una c�mara espec�fica y se escale en funci�n de una resoluci�n de referencia. Esto es 
/// necesario para cualquier canvas ya que la herramienta de Zoom lo requiere. Permite al desarrolador
/// trabajar con un canvas en modo Overlay facilmente sin preocuparse.
/// </summary>


[RequireComponent(typeof(Canvas))]  // Obligatorio que el GameObject sea un Canvas
public class AutoScaleCanvasToCamera : MonoBehaviour
{
    public Camera targetCamera; // C�mara objetivo frente a la cual se posicionar� el canvas
    public float distanceFromCamera = 2f; // Distancia a la que se colocar� el canvas respecto a la c�mara
    public Vector2 referenceResolution = new Vector2(1920, 1080); // Resoluci�n de referencia para el escalado del canvas

    private Canvas canvas; // Referencia a su componente Canvas

    void Start()
    {
        canvas = GetComponent<Canvas>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main; // Si no se asign� una c�mara, se usa la principal
        }

        // Configura el modo de renderizado como World Space
        canvas.renderMode = RenderMode.WorldSpace;

        if (canvas.worldCamera == null)
            canvas.worldCamera = targetCamera; // Asigna la c�mara objetivo si no se ha definido una c�mara mundial

        // Llama al m�todo que posiciona y escala el canvas
        PositionAndScaleCanvas();
    }

    void PositionAndScaleCanvas()
    {
        // Pone el canvas delante de la c�mara
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
        transform.rotation = targetCamera.transform.rotation;

        // Calcula la altura visible en unidades del mundo dependiendo del tipo de c�mara
        float screenHeightUnits;
        if (targetCamera.orthographic)
        {
            // Altura del �rea visible para c�mara ortogr�fica
            screenHeightUnits = targetCamera.orthographicSize * 2f;
        }
        else
        {
            // Para c�mara en perspectiva, se usa trigonometr�a para calcular la altura visible
            float fovInRadians = targetCamera.fieldOfView * Mathf.Deg2Rad;
            screenHeightUnits = 2f * distanceFromCamera * Mathf.Tan(fovInRadians / 2f);
        }

        // Calcula cu�ntas unidades del mundo equivale un p�xel
        float unitsPerPixel = screenHeightUnits / referenceResolution.y;

        // Calcula el tama�o del canvas en unidades del mundo
        float canvasWidth = referenceResolution.x * unitsPerPixel;
        float canvasHeight = referenceResolution.y * unitsPerPixel;

        RectTransform rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta = referenceResolution; // Establece el tama�o del rect�ngulo en p�xeles de referencia
        rt.localScale = Vector3.one * unitsPerPixel; // Escala el canvas para ajustarse al tama�o en unidades del mundo
    }
}