using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public RectTransform playerIcon;     // chấm đỏ
    public RectTransform mapRect;        // ảnh bản đồ nhỏ
    public Transform player;             // player thật trong scene
    public Vector2 worldSize;            // kích thước map thế giới

    void Update()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return; // Nếu không tìm thấy player thì thoát
        }
        // Tính tỉ lệ vị trí của Player trong bản đồ
        Vector2 normalized = new Vector2(
            player.position.x / worldSize.x,
            player.position.z / worldSize.y
        );

        // Tính kích thước minimap
        Vector2 mapSize = mapRect.sizeDelta;

        // Chuyển tỉ lệ sang vị trí UI
        Vector2 mapPos = new Vector2(
            normalized.x * mapSize.x,
            normalized.y * mapSize.y
        );

        // Cập nhật vị trí icon đỏ
        playerIcon.anchoredPosition = mapPos;
    }
}
