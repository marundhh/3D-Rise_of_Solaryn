
using UnityEngine.EventSystems;
using UnityEngine;

public class MinimapDragController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Camera bigMapCamera; // camera của big map
    public float dragSpeed = 0.001f;
    private Vector2 lastMousePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enabled || bigMapCamera == null) return;

        Vector2 currentMousePosition = eventData.position;
        Vector2 delta = currentMousePosition - lastMousePosition;

        Vector3 move = new Vector3(-delta.x * dragSpeed, 0, -delta.y * dragSpeed);
        bigMapCamera.transform.position += move;

        lastMousePosition = currentMousePosition;
    }
}
