using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class StoryExecutor : MonoBehaviour
{
    public EnemySpawnWave enemySpawnWave;
    public StoryStateData storyState;
    public static StoryExecutor Instance { get; private set; }
    [System.Serializable]
    public class IDObjectPair
    {
        public string id;
        public GameObject target;
    }

    public List<IDObjectPair> objectBindings;

    private Dictionary<string, GameObject> objectMap = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
                return;
        }
        Instance = this;
        foreach (var pair in objectBindings)
            objectMap[pair.id] = pair.target;

    }

    private void Start()
    {
        enemySpawnWave = FindAnyObjectByType<EnemySpawnWave>();
    }

     public void RestoreStoryState()
    {
        var json = PlayerPrefs.GetString("StoryState", "{}");
        if(string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("No story state found in PlayerPrefs.");
            return;
        }
        var state = JsonUtility.FromJson<StoryStateData>(json);
        storyState = state ?? new StoryStateData();
        Debug.Log("Restoring Story State");
        Debug.Log(json);
        foreach (var id in state.enabledObjects)
        {
            if (objectMap.TryGetValue(id, out var go))
                go.SetActive(true);
        }

        foreach (var id in state.disabledObjects)
        {
            if (objectMap.TryGetValue(id, out var go))
                go.SetActive(false);
        }

        foreach (var pair in state.movedObjects)
        {
            if (objectMap.TryGetValue(pair.objectID, out var go))
                go.transform.localPosition = pair.position;
        }

    }
    public void ClearStoryState()
    {
        Debug.Log("Clearing Story State");
        storyState = new StoryStateData();
        PlayerPrefs.DeleteKey("StoryState");
        PlayerPrefs.Save();
    }
    public async UniTask ExecuteBlock(CompositeStoryBlock block)
    {

        Debug.Log("start loop");
        foreach (var action in block.actions)
        {
            await ExecuteAction(action);
        }
    }
    public Transform GetTransfromByID(string ID)
    {
        if(string.IsNullOrEmpty(ID))
        {
            return null;
        }

        objectMap.TryGetValue(ID, out var go);
        if(go == null)
        {
            Debug.LogWarning("No GameObject found with ID: " + ID);
            return null;
        } else
            return go.transform;
     }

    private async UniTask ExecuteAction(StoryActionBlock action)
    {
        Debug.Log("start action");
        switch (action.action)
        {
            case StoryAction.EnableObject:
                if (objectMap.TryGetValue(action.targetID, out var go1))
                    go1.SetActive(true);
                storyState.enabledObjects.Add(action.targetID);
                break;

            case StoryAction.DisableObject:
                if (objectMap.TryGetValue(action.targetID, out var go2))
                    go2.SetActive(false);
                storyState.disabledObjects.Add(action.targetID);
                break;
            case StoryAction.MoveObject:
              //  Debug.Log("move object" + action.targetID);
                if (objectMap.TryGetValue(action.targetID, out var go3))
                {
                    go3.transform.localPosition = action.position;
                    storyState.movedObjects.Add(new MovedObjectData
                    {
                        objectID = action.targetID,
                        position = action.position
                    });
                }           
                    break;
            case StoryAction.Mission:
               // Debug.Log("new mision");
                var missionRaw = action.missionRaw;
                MissionManager.Instance.AcceptNewMission(missionRaw.missionType, missionRaw.id, 
                    missionRaw.tile, missionRaw.description, missionRaw.type, missionRaw.count, missionRaw.loacation, missionRaw.rewardGold, missionRaw.rewardExp, missionRaw.storyID);
                break;
            case StoryAction.Spawn:
                enemySpawnWave.EnqueueSpawn(action.prefabs, action.position, action.count);
                break;
            case StoryAction.NPCRelationShip:
                RelationshipManager.Instance.ModifyRelationship(action.targetID, action.relationShipEff);
                
                break;
            case StoryAction.NPCDialogue:
               DialogueBlockNpcHandler.Instance.SetNpcDialogueID(action.targetID, action.dialogID);
               break;
            case StoryAction.StoryIDSetup:
                GameFlowManager.Instance.SetupStory(action.storyID);
                break;
            case StoryAction.StoryIDSetupWithOutSave:
                GameFlowManager.Instance.CallSetupStoryWithOutSave(action.storyID);
                break;
            case StoryAction.Wait:
                SceneTransitionManager.Instance.SetNotification(action.Notification);
                await UniTask.Delay((int)(action.waitTime * 1000));
                break;
        }
    }

    public void SaveStoryState()
    {
        Debug.Log("Saving Story State");
        string json = JsonUtility.ToJson(storyState);
        Debug.Log(json);
        PlayerPrefs.SetString("StoryState", json);
        PlayerPrefs.Save();
    }
}

