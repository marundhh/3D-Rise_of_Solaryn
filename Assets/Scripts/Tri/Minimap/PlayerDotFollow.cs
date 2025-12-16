using UnityEngine;
using UnityEngine.UI;

public class PlayerDotFollow : MonoBehaviour
{
    public Transform player;                   // object nhân vật trong scene
    public RectTransform mapRect;             // ảnh bản đồ gán vào đây (RectTransform)
    public RectTransform playerIcon;          // chấm đỏ trong minimap

    public Vector2 worldMin = new Vector2(-250, -250); // Góc trái dưới của thế giới
    public Vector2 worldSize = new Vector2(500, 500);  // Kích thước thế giới (width, height)

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
            if (player == null) return; // Nếu không tìm thấy player thì thoát
        }
        // Lấy vị trí player hiện tại
        Vector2 worldPos = new Vector2(player.position.x, player.position.z);

        // Chuyển về tỉ lệ 0 -> 1
        Vector2 normalized = (worldPos - worldMin) / worldSize;

        // Chuyển sang vị trí trong ảnh minimap
        Vector2 mapSize = mapRect.sizeDelta;
        Vector2 mapPos = new Vector2(normalized.x * mapSize.x, normalized.y * mapSize.y);

        // Gán lại vị trí chấm đỏ
        playerIcon.anchoredPosition = mapPos;
    }
}
