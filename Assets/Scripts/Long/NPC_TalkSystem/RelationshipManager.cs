
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RelationshipManager : MonoBehaviour
{
    public static RelationshipManager Instance { get; private set; }
    [SerializeField] private Dictionary<string, int> relations = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
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
    public async Task SaveRelationship()
    {
        // Đọc dữ liệu cũ (nếu có)
        RelationshipSaveData oldData = await NpcData.instance.LoadRelationshipsAsync();

        // Chuyển dữ liệu cũ sang dictionary
        Dictionary<string, int> mergedData = new Dictionary<string, int>();
        foreach (var entry in oldData.entries)
        {
            mergedData[entry.npcName] = entry.value;
        }

        // Merge: thay đổi hoặc thêm mới từ relations hiện tại
        foreach (var pair in relations)
        {
            mergedData[pair.Key] = pair.Value; // nếu tồn tại sẽ ghi đè, nếu chưa có sẽ thêm
        }

        // Tạo saveData mới từ mergedData
        var saveData = new RelationshipSaveData();
        foreach (var pair in mergedData)
        {
            saveData.entries.Add(new RelationEntry
            {
                npcName = pair.Key,
                value = pair.Value
            });
        }

        await NpcData.instance.SaveRelationshipsAsync(saveData.entries);


    }

    public async Task LoadRelationship()
    {
        var saveData = await NpcData.instance.LoadRelationshipsAsync();

        relations.Clear();
        foreach (var entry in saveData.entries)
        {
            relations[entry.npcName] = entry.value;
        }
    }
    public async Task ResetRelationship()
    {
        // 1. Xóa dữ liệu trong bộ nhớ
        relations.Clear();

        // 2. Ghi đè dữ liệu rỗng vào PlayerPrefs
        var emptyData = new RelationshipSaveData(); // danh sách entries rỗng
        await NpcData.instance.SaveRelationshipsAsync(emptyData.entries);
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