using UnityEngine;
using UnityEngine.UI;

public class Map2SceneManager : MonoBehaviour
{
    public StaticBigMapController bigMapController; // script minimap chính
    public MapData mapData; // set cho từng Scene

    void Start()
    {
        LoadMap();
    }

    void LoadMap()
    {
        if (mapData != null && bigMapController != null)
        {
            // Đổi ảnh minimap
            bigMapController.mapRect.GetComponent<Image>().sprite = mapData.mapImage;

            // Đổi thông số
            bigMapController.worldSize = mapData.worldSize;
            bigMapController.worldOrigin = mapData.worldOrigin;
        }
    }
}
