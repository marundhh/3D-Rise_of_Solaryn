using UnityEngine;

public class StaticBigMapController : MonoBehaviour
{
    public RectTransform mapRect;          
    public RectTransform playerIcon;        
    public Transform player;                

    [Header("World Settings")]
    public Vector2 worldSize = new Vector2(1000, 1000);     
    public Vector2 worldOrigin = new Vector2(-1000, 2000);  

    void Update()
    {
        if (player == null)
        {
            var root = GameObject.Find("PlayerRoot");
            if (root != null) player = root.transform;
            return; 
        }

        float x = (player.position.x - worldOrigin.x) / worldSize.x;
        float y = (player.position.z - worldOrigin.y) / worldSize.y;

        x = Mathf.Clamp01(x);
        y = Mathf.Clamp01(y);

        Vector2 mapSize = mapRect.sizeDelta;

        Vector2 mapPos = new Vector2(
            (x - 0.35f) * mapSize.x,
            (y - 0.45f) * mapSize.y
        );

        playerIcon.anchoredPosition = mapPos;
    }
}
