using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NpcData : MonoBehaviour
{
    public static NpcData instance;

    public List<NpcDialogueEntry> npcDialogues = new List<NpcDialogueEntry>();
    public List<RelationEntry> relationships = new List<RelationEntry>();
    public List<MissionSaveData> missions = new List<MissionSaveData>();
    public StoryStateResponse storyState;
    public string currentStoryBlock;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // ==== NPC Dialogues ====
    public async Task<NpcDialogueSaveData> LoadNpcDialoguesAsync()
    {
        var tcs = new TaskCompletionSource<NpcDialogueSaveData>();

        ApiManager.Instance.GetNpcDialogues(
            (res) =>
            {
                npcDialogues = res.entries;
                tcs.TrySetResult(new NpcDialogueSaveData { entries = npcDialogues });
            },
            (err) =>
            {
                tcs.TrySetException(new Exception(err));
            }
        );

        return await tcs.Task;
    }

    public async Task SaveNpcDialoguesAsync(List<NpcDialogueEntry> newEntries)
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.SaveNpcDialogues(newEntries,
            (res) => { tcs.TrySetResult(res.message); },
            (err) => { tcs.TrySetException(new Exception(err)); }
        );

        await tcs.Task;
    }

    // ==== Relationships ====
    public async Task<RelationshipSaveData> LoadRelationshipsAsync()
    {
        var tcs = new TaskCompletionSource<RelationshipSaveData>();

        ApiManager.Instance.GetRelationships(
            (res) =>
            {
                relationships = res.entries;
                tcs.TrySetResult(new RelationshipSaveData { entries = relationships });
            },
            (err) =>
            {
                tcs.TrySetException(new Exception(err));
            }
        );

        return await tcs.Task;
    }

    public async Task SaveRelationshipsAsync(List<RelationEntry> newEntries)
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.SaveRelationships(newEntries,
            (res) => { tcs.TrySetResult(res.message); },
            (err) => { tcs.TrySetException(new Exception(err)); }
        );

        await tcs.Task;
    }

    // ==== Missions ====
    public async Task<List<MissionSaveData>> LoadMissionsAsync()
    {
        var tcs = new TaskCompletionSource<List<MissionSaveData>>();

        ApiManager.Instance.GetMissions(
            (res) =>
            {
                missions = res.missions;
                tcs.TrySetResult(missions);
            },
            (err) =>
            {
                tcs.TrySetException(new Exception(err));
            }
        );

        return await tcs.Task;
    }

    public async Task SaveMissionsAsync(List<MissionSaveData> newMissions)
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.SaveMissions(newMissions,
            (res) => { tcs.TrySetResult(res.message); },
            (err) => { tcs.TrySetException(new Exception(err)); }
        );

        await tcs.Task;
    }

    // ==== Story State ====
    public async Task<StoryStateResponse> LoadStoryStateAsync()
    {
        var tcs = new TaskCompletionSource<StoryStateResponse>();

        ApiManager.Instance.GetStoryState(
            (res) =>
            {
                storyState = res;
                tcs.TrySetResult(storyState);
            },
            (err) =>
            {
                tcs.TrySetException(new Exception(err));
            }
        );

        return await tcs.Task;
    }

    public async Task SaveStoryStateAsync(StoryStateResponse newState)
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.SaveStoryState(newState,
            (res) => { tcs.TrySetResult(res.message); },
            (err) => { tcs.TrySetException(new Exception(err)); }
        );

        await tcs.Task;
    }

    // ==== Current Story Block ====
    public async Task<string> LoadCurrentStoryBlockAsync()
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.GetCurrentStoryBlock(
            (res) =>
            {
                currentStoryBlock = res;
                tcs.TrySetResult(currentStoryBlock);
            },
            (err) =>
            {
                tcs.TrySetException(new Exception(err));
            }
        );

        return await tcs.Task;
    }

    public async Task SaveCurrentStoryBlockAsync(string blockId)
    {
        var tcs = new TaskCompletionSource<string>();

        ApiManager.Instance.SaveCurrentStoryBlock(blockId,
            (res) => { tcs.TrySetResult(res.message); },
            (err) => { tcs.TrySetException(new Exception(err)); }
        );

        await tcs.Task;
    }
}
