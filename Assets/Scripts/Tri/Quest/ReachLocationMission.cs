using System.Collections;
using UnityEngine;

public class ReachLocationMission : Mission
{
    public Transform targetPoint;
    public Transform playerPoint;

    private string targetLocation;
    public string LocationName => targetLocation;

    public ReachLocationMission(int id, string title, string description, string location,Transform targetPoint, int rewardGold, int rewardExp)
    {
        missionID = id;
        Title = title;
        Description = description;
        targetLocation = location;
        this.targetPoint = targetPoint;
        IsCompleted = false;
        this.rewardGold = rewardGold;
        this.rewardExp = rewardExp;
        playerPoint = GameObject.FindWithTag("Player").transform;
    }

    public override void OnEvent(GameEvent e)
    {
        if (e is ReachedLocationEvent le && le.locationName == targetLocation)
        {
            IsCompleted = true;

        }
    }

    public override string GetProgressText()
    {
        return IsCompleted ? $"Arrived at location {targetLocation}" : $"Need arrive at location <color=red>{targetLocation}</color>. ({(int)GetCurrentDistance()} m)";
      
    }

    public float GetCurrentDistance()
    {
        if (targetPoint == null || playerPoint == null) { return 0; }
        return Vector3.Distance(playerPoint.position, targetPoint.position);
    }
}
