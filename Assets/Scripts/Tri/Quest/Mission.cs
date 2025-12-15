using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public int missionID;
    public string Title;
    public string Description;
    public bool IsCompleted { get;  set; }

    protected int rewardGold = 0;
    protected int rewardExp = 0;

    protected string storyID = string.Empty;
    public abstract void OnEvent(GameEvent e);
    public abstract string GetProgressText();

    // ✅ Thêm phương thức này
    public virtual Transform GetTargetTransform()
    {
        return null; // Mặc định không có target
    }
    public int GetGoldReward() => rewardGold;
    public int GetExpReward() => rewardExp;
    public string GetStoryID() => storyID;
}
