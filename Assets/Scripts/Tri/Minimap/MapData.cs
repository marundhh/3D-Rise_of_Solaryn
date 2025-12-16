using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/Map Data", order = 0)]
public class MapData : ScriptableObject
{
    public Sprite mapImage;
    public Vector2 worldSize;
    public Vector2 worldOrigin;
}
