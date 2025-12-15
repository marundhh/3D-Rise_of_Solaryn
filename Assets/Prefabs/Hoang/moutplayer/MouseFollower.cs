using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public Camera mainCamera;
    public float distanceFromCamera = 5f;

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanceFromCamera;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        transform.position = worldPos;
    }
}
