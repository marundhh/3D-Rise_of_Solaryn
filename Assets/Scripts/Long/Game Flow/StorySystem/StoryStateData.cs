using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryStateData
{
    public List<string> disabledObjects = new();       // object đã bị disable
    public List<string> enabledObjects = new();        // object đã bật lên
    public List<MovedObjectData> movedObjects = new(); 
}
[System.Serializable]
public class MovedObjectData
{
    public string objectID;
    public Vector3 position;
    public Vector3 rotation;
}