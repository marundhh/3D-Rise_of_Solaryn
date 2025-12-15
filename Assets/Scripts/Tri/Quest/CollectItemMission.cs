public class CollectItemMission : Mission
{
    private string targetItem;
    private int requiredCount;
    private int currentCount;

    public string ItemType => targetItem;
    public int RequiredCount => requiredCount;
    public int CurrentCount => currentCount;

    public CollectItemMission(int id, string title, string description, string item, int count)
    {
        missionID = id;
        Title = title;
        Description = description;
        targetItem = item;
        requiredCount = count;
        currentCount = 0;
        IsCompleted = false;
    }

    public override void OnEvent(GameEvent e)
    {
        if (e is ItemCollectedEvent ie && ie.itemName == targetItem)
        {
            currentCount++;
            if (currentCount >= requiredCount)
                IsCompleted = true;
        }
    }

    public override string GetProgressText()
    {
        return $"{currentCount} / {requiredCount} {targetItem}s collected";
    }
}
