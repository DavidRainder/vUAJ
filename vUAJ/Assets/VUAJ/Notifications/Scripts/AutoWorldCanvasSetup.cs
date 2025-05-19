using System;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AutoScaleCanvasToCamera : MonoBehaviour
{
    public Camera targetCamera;
    public float distanceFromCamera = 2f;
    public Vector2 referenceResolution = new Vector2(1920, 1080);

    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        canvas.renderMode = RenderMode.WorldSpace;

        if (canvas.worldCamera == null)
            canvas.worldCamera = targetCamera;

        PositionAndScaleCanvas();
    }

    void PositionAndScaleCanvas()
    {
        // Pone el canvas delante de la cámara
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;
        transform.rotation = targetCamera.transform.rotation;

        // Calcula el tamaño visible en unidades del mundo
        float screenHeightUnits;
        if (targetCamera.orthographic)
        {
            screenHeightUnits = targetCamera.orthographicSize * 2f;
        }
        else
        {
            float fovInRadians = targetCamera.fieldOfView * Mathf.Deg2Rad;
            screenHeightUnits = 2f * distanceFromCamera * Mathf.Tan(fovInRadians / 2f);
        }

        float unitsPerPixel = screenHeightUnits / referenceResolution.y;
        float canvasWidth = referenceResolution.x * unitsPerPixel;
        float canvasHeight = referenceResolution.y * unitsPerPixel;

        RectTransform rt = canvas.GetComponent<RectTransform>();
        rt.sizeDelta = referenceResolution;
        rt.localScale = Vector3.one * unitsPerPixel;
    }
}