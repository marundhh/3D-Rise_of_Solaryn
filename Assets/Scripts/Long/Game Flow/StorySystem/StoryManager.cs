using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    [SerializeField] private List<CompositeStoryBlock> storyBlocks;
    [SerializeField] private StoryExecutor executor;

    private Dictionary<string, CompositeStoryBlock> blockMap = new();
    //private HashSet<string> playedBlocks = new(); // lưu trạng thái phân đoạn đã chơi


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var block in storyBlocks)
        {
          //  Debug.Log("Id:" + block.blockID);
            if (!string.IsNullOrEmpty(block.blockID))
                blockMap[block.blockID] = block;
        }
    }

    public async UniTask PlayBlock(string blockID)
    {

        if (!blockMap.TryGetValue(blockID, out var block))
        {
            Debug.LogWarning($"Không tìm thấy StoryBlock có ID: {blockID}");
            return;
        }

        Debug.Log("start Await Excute");
        await executor.ExecuteBlock(block); 
    }
}
