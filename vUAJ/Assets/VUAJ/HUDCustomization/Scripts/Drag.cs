using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Drag : MonoBehaviour, IDragHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject screenOverviewCanvas;
    private RectTransform rectTransform;
    private RectTransform canvasRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
    }
    void Start()
    {
        if (screenOverviewCanvas == null) screenOverviewCanvas = GameObject.FindGameObjectWithTag("overView");
        canvasRect = screenOverviewCanvas.GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Vector2 ClampToParent(Vector3 desiredPos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRect = screenOverviewCanvas.GetComponent<RectTransform>(); 
        Vector2 size = rectTransform.rect.size * rectTransform.localScale;

        Vector3[] canvasCorners = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3[] corners = new[] { new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f) };
        rectTransform.GetWorldCorners(corners);
        float localWidth =  (corners[3].x - corners[0].x) / 2; 
        float localHeight = (corners[1].y  - corners[0].y) / 2;

        Vector2 canvasSize = canvasRect.rect.size * canvasRect.localScale; 
        Vector2 offset = new Vector2(10, 10);

        float x = Mathf.Clamp(desiredPos.x, canvasCorners[0].x + localWidth , canvasCorners[3].x - localWidth);
        float y = Mathf.Clamp(desiredPos.y, canvasCorners[0].y + localHeight, canvasCorners[1].y - localHeight);

        return new Vector3(x, y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        canvasRect = screenOverviewCanvas.GetComponent<RectTransform>();
        Vector3 pos;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        transform.position = ClampToParent(pos);
        Debug.Log("eventDatapos: " + eventData.position);
        Debug.Log("newPos" + pos);
        RectTransform rectTransform = GetComponent<RectTransform>();
    }
}
