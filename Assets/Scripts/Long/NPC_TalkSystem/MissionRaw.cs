using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "missionRaw", menuName = "Mission/missionRaw")]
public class MissionRaw : ScriptableObject
{
    public MissionType missionType;
    public int id;
    public string tile;
    public string description;
    public string type;
    public int count;
    public string loacation;
    public int rewardGold;
    public int rewardExp;
    public string storyID;
}
