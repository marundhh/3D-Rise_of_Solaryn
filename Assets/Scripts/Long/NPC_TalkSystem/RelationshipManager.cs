
using UnityEngine;
using System.Collections.Generic;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance { get; private set; }
    [SerializeField] private Dictionary<string, int> relations = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetRelationship(string npcName)
    {
        if (!relations.ContainsKey(npcName)) relations[npcName] = 0;
        return relations[npcName];
    }
    public void ModifyRelationship(string npcName, int amount)
    {
        if (!relations.ContainsKey(npcName)) relations[npcName] = 0;
        relations[npcName] += amount;
    }
    public void SaveRelationship()
    {
        var saveData = new RelationshipSaveData();

        foreach (var pair in relations)
        {
            saveData.entries.Add(new RelationEntry
            {
                npcName = pair.Key,
                value = pair.Value
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("RELATIONSHIP_SAVE", json);
        PlayerPrefs.Save();
    }
    public void LoadRelationship()
    {
        Debug.Log("LoadRelationship");
        if (!PlayerPrefs.HasKey("RELATIONSHIP_SAVE")) return;

        string json = PlayerPrefs.GetString("RELATIONSHIP_SAVE");
        var saveData = JsonUtility.FromJson<RelationshipSaveData>(json);

        relations.Clear();
        foreach (var entry in saveData.entries)
        {
            relations[entry.npcName] = entry.value;
        }
    }
    public void ResetRelationship()
    {
        relations.Clear();
        SaveRelationship();
        Debug.Log("All relationships have been reset.");
    }
}
[System.Serializable]
public class RelationshipSaveData
{
    public List<RelationEntry> entries = new();
}

[System.Serializable]
public class RelationEntry
{
    public string npcName;
    public int value;
}