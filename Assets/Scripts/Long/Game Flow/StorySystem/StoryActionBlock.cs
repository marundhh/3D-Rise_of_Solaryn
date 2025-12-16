using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class StoryActionBlock
{
    public StoryAction action;
    public GameObject prefabs;
    public int count;
    public string targetID;
    public Vector3 position;
    public Vector3 rotation;
    public MissionRaw missionRaw;
    public int relationShipEff;
    public string dialogID;
    public string storyID;
    public float waitTime;
    public string Notification;
    public Vector3 cameraTargetPosition;      // Vị trí di chuyển đến
    public Vector3 cameraLookOffset;          // Hướng nhìn lệch (ví dụ: nhìn sang trái/phải)
    public float cameraMoveDuration = 2f;     // Thời gian di chuyển camera
    public float cameraLookDuration = 1f;
    public PlayableAsset cutSceneAsset;
    public string sceneName;
    public int minusCoins;
}
