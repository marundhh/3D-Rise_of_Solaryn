using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 5f;
    public float maxRotationY = 45f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);
            Vector3 direction = lookPoint - transform.position;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion limitedRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

                // Giới hạn góc quay Y nếu cần
                Vector3 angles = limitedRotation.eulerAngles;
                //angles.x = 0; // Giữ đầu không ngửa/cúi
                angles.z = 0;

                transform.rotation = Quaternion.Euler(angles);
            }
        }
    }
}
