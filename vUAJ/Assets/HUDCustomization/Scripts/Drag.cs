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

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvas = GetComponentInParent<Canvas>();
    }
    void Start()
    {
        if (screenOverviewCanvas == null) screenOverviewCanvas = GameObject.FindGameObjectWithTag("overView");
        canvasRect = screenOverviewCanvas.GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Vector2 ClampToParent(Vector3 desiredPos)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform canvasRect = screenOverviewCanvas.GetComponent<RectTransform>(); //* rectTransform.localScale.x * transform.parent.GetComponent<RectTransform>().localScale.x,
        Vector2 size = new Vector2(rectTransform.rect.width, rectTransform.rect.height );
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 offset = new Vector2(10, 10);
        float x = Mathf.Clamp(desiredPos.x, -canvasSize.x / 2 + offset.x + size.x / 2, canvasSize.x / 2 - offset.x - size.x / 2);
        float y = Mathf.Clamp(desiredPos.y, -canvasSize.y / 2+ offset.y + size.y / 2, canvasSize.y / 2 - offset.y - size.y / 2);

        return new Vector3(x, y, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector2 pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    (RectTransform)screenOverviewCanvas.transform, pointerData.position, screenOverviewCanvas.worldCamera, out pos);
        canvasRect = screenOverviewCanvas.GetComponent<RectTransform>();
        Vector3 pos;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRect,
            eventData.position-new Vector2(Screen.width/2, Screen.height/2),
            Camera.current,
            out pos
        );

        Debug.Log("eventDatapos: " + eventData.position);
        Debug.Log("newPos" + pos);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D = ClampToParent(pos);
       //rectTransform.anchoredPosition = pos;
        //rectTransform.anchoredPosition3D = pos;
        //transform.SetPositionAndRotation(pos, Quaternion.identity);
    }
}
