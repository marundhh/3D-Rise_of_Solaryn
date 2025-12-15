using UnityEngine;

[System.Serializable]
public class DialogueCondition
{
    public int requiredQuestID = -1;
    public bool requireQuestCompleted = false;
    public int requiredRelationship = int.MinValue;
    public string requiredWorldFlag;

    public bool IsMet(string npcName)
    {
        if (requiredQuestID >= 0)
        {
            var quest = MissionManager.Instance.GetMissionByID(requiredQuestID);
            Debug.Log(quest);
            if (quest == null) return false;
            if (requireQuestCompleted && !quest.IsCompleted) return false;
        }
        int rel = RelationshipManager.Instance.GetRelationship(npcName);
        if (requiredRelationship != int.MinValue && rel < requiredRelationship) return false;
        return true;

    }
}