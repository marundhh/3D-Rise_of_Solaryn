using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public GameObject miniMapSmall;
    public GameObject miniMapBig;
    public Camera miniMapCamera;
    public MiniMapFollow miniMapFollow;
    public MinimapDragController dragController; // Gán trong Inspector

    private bool isBigMap = false;

    void Start()
    {
        if (miniMapSmall == null || miniMapBig == null || miniMapCamera == null || miniMapFollow == null || dragController == null)
        {
            Debug.LogError("⚠️ MiniMapManager: Chưa gán đủ đối tượng trong Inspector!");
            return;
        }

        miniMapSmall.SetActive(true);
        miniMapBig.SetActive(false);
        isBigMap = false;

        dragController.enabled = false; // Tắt kéo khi bắt đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isBigMap = !isBigMap;
            ToggleMiniMap();
        }
    }

    void ToggleMiniMap()
    {
        miniMapSmall.SetActive(!isBigMap);
        miniMapBig.SetActive(isBigMap);

        miniMapCamera.orthographicSize = isBigMap ? 50f : 10f;
        miniMapFollow.SetFollow(!isBigMap);
        dragController.enabled = isBigMap; // Bật kéo khi mở map lớn

        Debug.Log($"📌 MiniMap: {(isBigMap ? "Toàn màn hình" : "Nhỏ")}");
    }
}
