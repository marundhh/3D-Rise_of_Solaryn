using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 15f;

    private CinemachineTransposer transposer;
    private float currentDistance;

    private void Start()
    {
        if (virtualCamera != null)
        {
            // Lấy component Transposer từ camera
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                // Lưu khoảng cách ban đầu
                currentDistance = -transposer.m_FollowOffset.z;
            }
        }
    }

    private void Update()
    {
        if (transposer == null) return;

        // Lấy giá trị lăn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Cập nhật khoảng cách theo lăn chuột
            currentDistance -= scroll * sensitivity;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

            // Cập nhật lại offset Z để zoom
            Vector3 offset = transposer.m_FollowOffset;
            offset.z = -currentDistance; // Z âm vì camera thường nhìn từ sau
            transposer.m_FollowOffset = offset;
        }
    }
}
