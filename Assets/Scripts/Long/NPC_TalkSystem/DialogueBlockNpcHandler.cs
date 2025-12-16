using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        Init();
    }

    public void Init()
    {
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
    //    Debug.Log($"GetDialogueID called for NPC: {npcName}");
        if (npcDialogueID.TryGetValue(npcName, out var set))
        {
            return set;
        }
        return null;
    }
    public async Task SaveDialogueProgress()
    {
        // Đọc dữ liệu cũ (nếu có)
        NpcDialogueSaveData oldData = await NpcData.instance.LoadNpcDialoguesAsync();

        // Chuyển dữ liệu cũ sang dictionary
        Dictionary<string, string> mergedData = new Dictionary<string, string>();
        foreach (var entry in oldData.entries)
        {
            mergedData[entry.npcName] = entry.dialogueID;
        }

        // Merge: thay đổi hoặc thêm mới từ npcDialogueID hiện tại
        foreach (var pair in npcDialogueID)
        {
           // Debug.Log($"Merging NPC: {pair.Key}, Dialogue ID: {pair.Value}");
            mergedData[pair.Key] = pair.Value; // nếu tồn tại sẽ ghi đè, nếu chưa có sẽ thêm
        }

        // Tạo saveData mới từ mergedData
        var saveData = new NpcDialogueSaveData();
        foreach (var pair in mergedData)
        {
          //  Debug.Log($"Saving NPC: {pair.Key}, Dialogue ID: {pair.Value}");
            saveData.entries.Add(new NpcDialogueEntry
            {
                npcName = pair.Key,
                dialogueID = pair.Value
            });
        }

       await NpcData.instance.SaveNpcDialoguesAsync(saveData.entries);

    }


    public async Task LoadDialogueProgress()
    {
        Debug.Log("Loading NPC dialogues from API...");
        // Gọi API để lấy dữ liệu
        var saveData = await NpcData.instance.LoadNpcDialoguesAsync();

        if (saveData.entries.Count != 0)
        {
            npcDialogueID.Clear();
            foreach (var entry in saveData.entries)
            {
                npcDialogueID[entry.npcName] = entry.dialogueID;
            }
        }
        else
        {
            Debug.Log("No saved dialogue data found, initializing with default values.");
        }
    }
    public async Task ResetDialogueProgress()
    {
        Debug.Log("ResetDialogueProgress called");
        // 1. Xóa dữ liệu trong bộ nhớ
        npcDialogueID.Clear();

        // 2. Ghi đè dữ liệu rỗng vào PlayerPrefs
        var emptyData = new NpcDialogueSaveData(); // entries rỗng

       await NpcData.instance.SaveNpcDialoguesAsync(emptyData.entries);

        Debug.Log("All dialogue progress has been reset (empty data saved).");


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