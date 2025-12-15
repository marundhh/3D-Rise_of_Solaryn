using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StoryActionBlock
{
    public StoryAction action;
    public GameObject prefabs;
    public int count;
    public string targetID;
    public Vector3 position;
    public MissionRaw missionRaw;
    public int relationShipEff;
    public string dialogID;
    public string storyID;
    public float waitTime;
    public string Notification;
}
