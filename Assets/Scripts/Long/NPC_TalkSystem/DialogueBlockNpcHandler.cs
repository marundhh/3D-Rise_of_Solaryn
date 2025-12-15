using UnityEngine;
using System.Collections.Generic;

public class DialogueBlockNpcHandler : MonoBehaviour
{
    public static DialogueBlockNpcHandler Instance { get; private set; }

    // Gán trong Inspector hoặc tự động load từ Resources
    public List<NPCDialogueSet> allNpcDialogueSets;

    private Dictionary<string, NPCDialogueSet> npcSetDict;
    private Dictionary<string, string> npcDialogueID = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        npcSetDict = new Dictionary<string, NPCDialogueSet>();
        foreach (var set in allNpcDialogueSets)
        {
            if (!npcSetDict.ContainsKey(set.npcName))
                npcSetDict.Add(set.npcName, set);
        }

        // Nếu chưa có NPC nào thì gán mặc định
        foreach (var key in npcSetDict.Keys)
        {
            if (!npcDialogueID.ContainsKey(key))
                npcDialogueID.Add(key, "start");
        }
    }

    public void SetNpcDialogueID(string npcName, string dialogueID)
    {
        if (npcDialogueID.ContainsKey(npcName))
        {
            npcDialogueID[npcName] = dialogueID;
        }

    }


    public string GetDialogueID(string npcName)
    {
        if (npcDialogueID.TryGetValue(npcName, out var set))
        {
            return set;
        }
        return null;
    }
    public void SaveDialogueProgress()
    {
        var saveData = new NpcDialogueSaveData();

        foreach (var pair in npcDialogueID)
        {
            saveData.entries.Add(new NpcDialogueEntry
            {
                npcName = pair.Key,
                dialogueID = pair.Value
            });
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString("NPC_DIALOGUE_SAVE", json);
        PlayerPrefs.Save();
    }

    public void LoadDialogueProgress()
    {
        Debug.Log("LoadDialogueProgress called");
        if (!PlayerPrefs.HasKey("NPC_DIALOGUE_SAVE")) return;

        string json = PlayerPrefs.GetString("NPC_DIALOGUE_SAVE");
        var saveData = JsonUtility.FromJson<NpcDialogueSaveData>(json);
        npcDialogueID.Clear();

        foreach (var entry in saveData.entries)
        {
            npcDialogueID[entry.npcName] = entry.dialogueID;
        }
    }
    public void ResetDialogueProgress()
    {
        Debug.Log("ResetDialogueProgress called");
        npcDialogueID.Clear();
        foreach (var key in npcSetDict.Keys)
        {
            npcDialogueID[key] = "start"; // Hoặc ID mặc định khác
        }
        SaveDialogueProgress();
    }
}



[System.Serializable]
public class NpcDialogueSaveData
{
    public List<NpcDialogueEntry> entries = new();
}

[System.Serializable]
public class NpcDialogueEntry
{
    public string npcName;
    public string dialogueID;
}