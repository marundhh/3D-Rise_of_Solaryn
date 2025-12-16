using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour
{
    public Transform playerRoot;
    public float defaultDistance = 6f;
    public float height = 0f; // KHÔNG dùng height nữa vì đã dùng góc pitch
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float rotationSpeed = 120f;
    public float rotateStep = 45f;
    public float pitchAngle = 45f; // Góc nghiêng từ trên xuống

    private float currentDistance;
    private float targetYaw = 0f;
    private float smoothYaw = 0f;
    private float yawVelocity;
    private Vector3 currentVelocity;

    void Start()
    {
        currentDistance = defaultDistance;
        targetYaw = playerRoot.eulerAngles.y;
        smoothYaw = targetYaw;
    }

    void LateUpdate()
    {
        if (playerRoot == null) return;


        // Smooth xoay ngang
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, targetYaw, ref yawVelocity, 0.1f);

        // Tạo góc quay có cả pitch (nghiêng xuống) và yaw (quay ngang)
        Quaternion rotation = Quaternion.Euler(pitchAngle, smoothYaw, 0);

        // Offset camera nghiêng từ trên xuống
        Vector3 offset = rotation * new Vector3(0, 0, -currentDistance);

        // Tính vị trí mong muốn
        Vector3 desiredPosition = playerRoot.position + offset;

        // Di chuyển camera mượt
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, 0.1f);

        // Nhìn về player (hơi lệch lên để thấy phần đầu)
        //transform.LookAt(playerRoot.position + Vector3.up * 1.5f);
    }


}
  
