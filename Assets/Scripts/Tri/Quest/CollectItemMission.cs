using UnityEngine;

public class CollectItemMission : Mission
{
    private string targetItem;
    private int requiredCount;
    private int currentCount;

    public Transform targetPoint;
    public Transform playerPoint;
    public string ItemType => targetItem;
    public int RequiredCount => requiredCount;
    public int CurrentCount => currentCount;

    public CollectItemMission(int id, string title, string description, string item, int count, Transform lacation, int rewardGold, int rewardExp, string storyID, int currentCount)
    {
        missionID = id;
        Title = title;
        Description = description;
        targetItem = item;
        requiredCount = count;
        this.currentCount = currentCount;
        currentCount = 0;
        IsCompleted = false;
        targetPoint = lacation;
        playerPoint = GameObject.FindWithTag("Player").transform;
        this.rewardGold = rewardGold;
        this.rewardExp = rewardExp;
        this.storyID = storyID;
    }

    public override void OnEvent(GameEvent e)
    {
        if (e is ItemCollectedEvent ie && ie.itemName == targetItem)
        {
            currentCount++;
            GameStateManager.Instance.SaveGame();
            if (currentCount >= requiredCount)
            {
                OnMissionComplete();
            }
        }
    }


    public override string GetProgressText()
    {
        return $"{currentCount} / {requiredCount} <color=red>{targetItem}s</color> collected  ({(int)GetCurrentDistance()} m)";
    }
    public float GetCurrentDistance()
    {
        if (targetPoint == null || playerPoint == null) { return 0; }
        return Vector3.Distance(playerPoint.position, targetPoint.position);
    }
}
