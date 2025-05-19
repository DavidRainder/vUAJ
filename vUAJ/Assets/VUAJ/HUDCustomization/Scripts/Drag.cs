using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

[Tooltip("Habilita el arrastre del objeto dentro de un panel que representa la pantalla de juego.")]
public class Drag : MonoBehaviour, IDragHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField, Tooltip("Panel representaci�n de la pantalla de juego")]
    private GameObject screenOverviewPanel;
    private RectTransform rectTransform; // Del objeto
    private RectTransform panelRect; // Del panel - pantalla
    private Canvas parentCanvas; // Canvas padre de panel

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    void Start()
    {
        if (screenOverviewPanel == null) screenOverviewPanel = GameObject.FindGameObjectWithTag("overView");
        panelRect = screenOverviewPanel.GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Mantiene la posici�n a mover el objeto dentro de los limites de la representaci�n de la pantalla.
    /// </summary>
    /// <param name="desiredPos"></param>
    /// <returns></returns>
    private Vector2 ClampToParent(Vector3 desiredPos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRect = screenOverviewPanel.GetComponent<RectTransform>(); // Antes era un canvas
        Vector2 size = rectTransform.rect.size * rectTransform.localScale;

        // Obtenci�n de l�mites del panel 
        Vector3[] canvasCorners = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
        canvasRect.GetWorldCorners(canvasCorners);

        // Obtenci�n de l�mites del objeto
        Vector3[] corners = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
        rectTransform.GetWorldCorners(corners);

        // Sizes para c�lculos y offset con l�mite de panel
        float localWidth =  (corners[3].x - corners[0].x) / 2; 
        float localHeight = (corners[1].y  - corners[0].y) / 2;
        Vector2 canvasSize = canvasRect.rect.size * canvasRect.localScale; 
        Vector2 offset = new Vector2(10, 10);

        float x = Mathf.Clamp(desiredPos.x, canvasCorners[0].x + localWidth , canvasCorners[3].x - localWidth);
        float y = Mathf.Clamp(desiredPos.y, canvasCorners[0].y + localHeight, canvasCorners[1].y - localHeight);

        return new Vector3(x, y, 0);
    }

    /// <summary>
    /// Arrastra el objeto dentro de los l�mites de la pantalla l�gica seg�n un punto.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // Conversi�n del punto a panel
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            panelRect,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        // Comprobaci�n de l�mites y asignaci�n 
        transform.position = ClampToParent(pos);
    }
}
