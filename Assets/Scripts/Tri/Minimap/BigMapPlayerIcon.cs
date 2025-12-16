using UnityEngine;

public class BigMapPlayerIcon : MonoBehaviour
{
    public RectTransform mapRect;     // RectTransform của BigMapPanel
    public RectTransform playerIcon;  // Icon đại diện người chơi (Image nhỏ)
    public Transform player;          // Transform người chơi
    public Vector2 worldSize;         // Kích thước thế giới (X: width, Y: height)

    void Update()
    {
        Vector2 normalized = new Vector2(
            player.position.x / worldSize.x,
            player.position.z / worldSize.y
        );

        Vector2 mapSize = mapRect.sizeDelta;
        Vector2 iconPos = new Vector2(
            normalized.x * mapSize.x,
            normalized.y * mapSize.y
        );

        // Đặt vị trí icon
        playerIcon.anchoredPosition = iconPos;
    }
}
