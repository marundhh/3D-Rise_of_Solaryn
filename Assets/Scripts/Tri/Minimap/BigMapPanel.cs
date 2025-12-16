using UnityEngine;
using UnityEngine.EventSystems;

public class BigMapDragController : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform mapContent;
    private Vector2 lastMousePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapContent, eventData.position, eventData.pressEventCamera, out lastMousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapContent, eventData.position, eventData.pressEventCamera, out currentMousePos);

        Vector2 delta = currentMousePos - lastMousePosition;
        mapContent.anchoredPosition += delta;
        lastMousePosition = currentMousePos;
    }
}
