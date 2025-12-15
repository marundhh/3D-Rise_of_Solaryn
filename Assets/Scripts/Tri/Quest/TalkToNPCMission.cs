using System.Collections;
using UnityEngine;

public class TalkToNPCMission : Mission
{

    public Transform targetPoint;
    public Transform playerPoint;

    private string npcName;
    public string NPCName => npcName; // ✅ Thêm dòng này

    public TalkToNPCMission(int id, string title, string description, string npc, Transform Location, int rewardGold, int rewardExp) 
    {
        missionID = id;
        Title = title;
        Description = description;
        npcName = npc;
        targetPoint = Location;
        playerPoint = GameObject.FindWithTag("Player").transform;
        IsCompleted = false;
        this.rewardGold = rewardGold;
        this.rewardExp = rewardExp;
    }

    public override void OnEvent(GameEvent e)
    {
        if (e is TalkToNPCEvent ev && ev.npcName == npcName)
        {
            IsCompleted = true;
        }
    }

    public override string GetProgressText()
    {
        return IsCompleted ? $"Spoke with {npcName}" : $"Need to talking with <color=red>{npcName}</color>. ({(int)GetCurrentDistance()} m)";
    }

    public float GetCurrentDistance()
    {
        if(targetPoint == null || playerPoint == null){ return 0;  }
        return Vector3.Distance(playerPoint.position, targetPoint.position);
    }
}
