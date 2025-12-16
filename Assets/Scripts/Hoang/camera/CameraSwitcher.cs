using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // thêm để bắt sự kiện scene load

public class CameraController : MonoBehaviour
{
    [Header("Cinemachine Setup")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float minHeight = 5f;
    [SerializeField] private float maxHeight = 25f;
    private float currentHeight;

    [Header("Rotation Settings")]
    private Vector3[] followOffsets = new Vector3[]
    {
        new Vector3(  0f, 14f, -10f),
        new Vector3(-10f, 14f,   0f),
        new Vector3(  0f, 14f,  10f),
        new Vector3( 10f, 14f,   0f),
    };
    private int currentIndex = 0;
    private Vector3 targetOffset;
    [SerializeField] private float smoothSpeed = 4f;

    [Header("Mouse Drag Settings")]
    [SerializeField] private float dragThreshold = 100f;
    private float startMouseX;
    private bool isDragging = false;
    private bool hasRotated = false;

    private CinemachineTransposer transposer;

    private void Awake()
    {
        // Đảm bảo camera không bị tạo trùng khi load scene
        var existing = FindObjectsOfType<CameraController>();
        if (existing.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // lắng nghe sự kiện load scene
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Start()
    {
        InitCamera();
    }

    public void InitCamera()
    {
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                targetOffset = followOffsets[currentIndex];
                currentHeight = targetOffset.y;
                transposer.m_FollowOffset = targetOffset;
            }
        }

        // Gán lại player khi khởi tạo
        AssignPlayerTarget();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Khi scene mới load xong, gán lại player cho camera
        AssignPlayerTarget();
    }

    private void AssignPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    public void ResetCamera()
    {
        if (transposer != null)
        {
            virtualCamera.Priority = 10;
        }
    }

    private void Update()
    {
        if (transposer == null) return;

        HandleMouseRotation();
        HandleZoomInput();

        Vector3 currentOffset = transposer.m_FollowOffset;
        Vector3 newOffset = Vector3.Lerp(
            currentOffset,
            new Vector3(targetOffset.x, currentHeight, targetOffset.z),
            Time.deltaTime * smoothSpeed
        );
        transposer.m_FollowOffset = newOffset;
    }

    private void HandleMouseRotation()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            hasRotated = false;
            startMouseX = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging && !hasRotated)
        {
            float deltaX = Input.mousePosition.x - startMouseX;

            if (Mathf.Abs(deltaX) > dragThreshold)
            {
                if (deltaX > 0)
                    currentIndex = (currentIndex + 1) % followOffsets.Length;
                else
                    currentIndex = (currentIndex - 1 + followOffsets.Length) % followOffsets.Length;

                SetTargetOffset(followOffsets[currentIndex]);
                hasRotated = true;
            }
        }
    }

    private void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentHeight -= scroll * sensitivity;
            currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);

            float t = Mathf.InverseLerp(minHeight, maxHeight, currentHeight);
            float maxZ = 14f;
            float minZ = 5f;
            float newZ = Mathf.Lerp(minZ, maxZ, t);

            Vector3 direction = new Vector3(targetOffset.x, 0f, targetOffset.z).normalized;
            targetOffset = new Vector3(direction.x * newZ, currentHeight, direction.z * newZ);
        }
    }

    private void SetTargetOffset(Vector3 offset)
    {
        targetOffset = offset;
    }
}
