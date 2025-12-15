using UnityEngine;
public class KillEnemyMission : Mission
{
    private string targetEnemyType;
    private int requiredKillCount;
    private int currentKillCount;

    public Transform targetPoint;
    public Transform playerPoint;
    public string EnemyType => targetEnemyType;
    public int RequiredKillCount => requiredKillCount;
    public int CurrentKillCount => currentKillCount;

    public KillEnemyMission(int id,string title, string description, string enemyType, int count, Transform lacation, int rewardGold, int rewardExp, string storyID)
    {
        missionID = id;
        Title = title;
        Description = description;
        targetEnemyType = enemyType;
        requiredKillCount = count;
        currentKillCount = 0;
        targetPoint = lacation;
        playerPoint = GameObject.FindWithTag("Player").transform;
        IsCompleted = false;
        this.rewardGold = rewardGold;
        this.rewardExp = rewardExp;
        this.storyID = storyID;

    }

    public override void OnEvent(GameEvent e)
    {
        if (e is EnemyKilledEvent ek && ek.enemyType == targetEnemyType)
        {
            currentKillCount++;
            if (currentKillCount >= requiredKillCount)
            {
                IsCompleted = true;
                if (!string.IsNullOrEmpty(storyID))
                    CallSetupStory();
            }      
        }
    }

    public async void CallSetupStory()
    {
        await GameFlowManager.Instance.CallSetupStoryWithOutSave(storyID);
    }

    public override string GetProgressText()
    {
        return $"{currentKillCount} / {requiredKillCount} <color=red>{targetEnemyType}s</color> defeated  ({(int)GetCurrentDistance()} m)";
    }
    public float GetCurrentDistance()
    {
        if (targetPoint == null || playerPoint == null) { return 0; }
        return Vector3.Distance(playerPoint.position, targetPoint.position);
    }
}
