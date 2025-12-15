// GameEvents.cs

public abstract class GameEvent { }

// Đã có: EnemyKilledEvent
public class EnemyKilledEvent : GameEvent
{
    public string enemyType;
    public EnemyKilledEvent(string type) => enemyType = type;
}

// ✅ Bổ sung: ItemCollectedEvent
public class ItemCollectedEvent : GameEvent
{
    public string itemName;
    public ItemCollectedEvent(string name) => itemName = name;
}

// ✅ Bổ sung: TalkToNPCEvent
public class TalkToNPCEvent : GameEvent
{
    public string npcName;
    public TalkToNPCEvent(string name) => npcName = name;
}

// ✅ Bổ sung: ReachedLocationEvent
public class ReachedLocationEvent : GameEvent
{
    public string locationName;
    public ReachedLocationEvent(string name) => locationName = name;
}
