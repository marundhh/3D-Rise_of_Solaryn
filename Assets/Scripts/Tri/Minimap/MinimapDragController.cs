using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapDragController : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform mapContent; // ảnh bản đồ
    public RectTransform maskArea;   // panel cha

    public float smoothSpeed = 10f;

    private Vector2 lastPointerPosition;
    private Vector2 targetPosition;  // vị trí mong muốn
    private bool isDragging;

    private void Start()
    {
        targetPosition = mapContent.anchoredPosition;
    }

    private void Update()
    {
        if (!isDragging) return;

        // Nội suy để di chuyển mượt
        mapContent.anchoredPosition = Vector2.Lerp(mapContent.anchoredPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(maskArea, eventData.position, eventData.pressEventCamera, out lastPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(maskArea, eventData.position, eventData.pressEventCamera, out currentPointerPosition))
        {
            Vector2 delta = currentPointerPosition - lastPointerPosition;
            targetPosition += delta;
            lastPointerPosition = currentPointerPosition;

            ClampTargetPosition();
        }
    }

    private void ClampTargetPosition()
    {
        Vector2 contentSize = mapContent.sizeDelta;
        Vector2 maskSize = maskArea.sizeDelta;

        float minX = maskSize.x - contentSize.x;
        float minY = maskSize.y - contentSize.y;

        if (contentSize.x <= maskSize.x) minX = 0;
        if (contentSize.y <= maskSize.y) minY = 0;

        float clampedX = Mathf.Clamp(targetPosition.x, minX, 0);
        float clampedY = Mathf.Clamp(targetPosition.y, minY, 0);

        targetPosition = new Vector2(clampedX, clampedY);
    }

    private void OnEndDrag()
    {
        isDragging = false;
    }
}
