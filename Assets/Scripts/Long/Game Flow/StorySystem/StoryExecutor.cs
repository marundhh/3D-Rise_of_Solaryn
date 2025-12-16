using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Playables;

public class StoryExecutor : MonoBehaviour
{
    public EnemySpawnWave enemySpawnWave;
    public StoryStateData storyState;
    public PlayableDirector playableDirector;

    public static StoryExecutor Instance { get; private set; }
    [System.Serializable]
    public class IDObjectPair
    {
        public string id;
        public GameObject target;
    }

    public List<IDObjectPair> objectBindings;
    public List<Playable> playables;

    private Dictionary<string, GameObject> objectMap = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Init();
    }

    public void Init()
    {
        foreach (var pair in objectBindings)
            objectMap[pair.id] = pair.target;
    }
    private void Start()
    {
        enemySpawnWave = FindAnyObjectByType<EnemySpawnWave>();

        objectMap.Add("Player", GameObject.FindGameObjectWithTag("Player"));
    }

    public async Task LoadStoryState()
    {
        var state = await NpcData.instance.LoadStoryStateAsync();

        List<MovedObjectData> movedObjectsTemp = new List<MovedObjectData>();

        foreach (var pair in state.movedObjects)
        {
            movedObjectsTemp.Add(new MovedObjectData
            {
                objectID = pair.objectID,
                position = new Vector3(pair.positionX, pair.positionY, pair.positionZ),
            });
            Debug.Log("Move object: " + pair.objectID + " to position: " + new Vector3(pair.positionX, pair.positionY, pair.positionZ));
        }

        StoryStateData storyStateTemp = new StoryStateData()
        {
            disabledObjects = state.disabledObjects,
            enabledObjects = state.enabledObjects,
            movedObjects = movedObjectsTemp
        };

        storyState = storyStateTemp ?? new StoryStateData();

        foreach (var id in storyState.enabledObjects)
        {
            if (objectMap.TryGetValue(id, out var go))
                go.SetActive(true);
            Debug.Log("Enable object: " + id);
        }

        foreach (var id in storyState.disabledObjects)
        {
            if (objectMap.TryGetValue(id, out var go))
                go.SetActive(false);
            Debug.Log("Disable object: " + id);
        }

        foreach (var pair in storyState.movedObjects)
        {
            if (objectMap.TryGetValue(pair.objectID, out var go))
                go.transform.localPosition = pair.position;
            Debug.Log("Move object: " + pair.objectID + " to position: " + pair.position);
        }

    }
    public async Task ClearStoryState()
    {
        // 1. Tạo dữ liệu rỗng trong bộ nhớ
        storyState = new StoryStateData();

        // 2. Chuyen dữ liệu rỗng sang định dạng StoryStateResponse
        List<StoryMovedObject> movedObjects = new List<StoryMovedObject>();
        foreach (var moved in storyState.movedObjects)
        {
            movedObjects.Add(new StoryMovedObject
            {
                objectID = moved.objectID,
                positionX = moved.position.x,
                positionY = moved.position.y,
                positionZ = moved.position.z,
            });
        }

        StoryStateResponse storyStateResponse = new StoryStateResponse
        {
            disabledObjects = storyState.disabledObjects,
            enabledObjects = storyState.enabledObjects,
            movedObjects = movedObjects
        };
        // 3. Ghi đè dữ liệu rỗng vào PlayerPrefs
        await NpcData.instance.SaveStoryStateAsync(storyStateResponse);
        Debug.Log("Story State cleared successfully");
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
        if (string.IsNullOrEmpty(ID))
        {
            return null;
        }

        objectMap.TryGetValue(ID, out var go);
        if (go == null)
        {
            Debug.LogWarning("No GameObject found with ID: " + ID);
            return null;
        }
        else
            return go.transform;
    }

    private async UniTask ExecuteAction(StoryActionBlock action)
    {
        Debug.Log("start action");
        Debug.Log("Executing action: " + action.action + " on target: " + action.targetID);
        switch (action.action)
        {
            case StoryAction.EnableObject:
                if (objectMap.TryGetValue(action.targetID, out var go1))
                    go1.SetActive(true);
                storyState.enabledObjects.Add(action.targetID);
                Debug.Log("Enable object: " + action.targetID);
                break;

            case StoryAction.DisableObject:
                if (objectMap.TryGetValue(action.targetID, out var go2))
                    go2.SetActive(false);
                storyState.disabledObjects.Add(action.targetID);
                Debug.Log("Disable object: " + action.targetID);
                break;
            case StoryAction.MoveObject:
                if (objectMap.TryGetValue(action.targetID, out var go3))
                {
                    go3.transform.localPosition = action.position;
                    storyState.movedObjects.Add(new MovedObjectData
                    {
                        objectID = action.targetID,
                        position = action.position
                    });
                    Debug.Log("move object" + action.targetID);
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
                GameFlowManager.Instance.CallSetupStoryNoneTransiton(action.storyID);
                break;
            case StoryAction.StoryIDSetupWithOutSave:
                GameFlowManager.Instance.CallSetupStoryWithOutSave(action.storyID);
                break;
            case StoryAction.Wait:
                SceneTransitionManager.Instance.SetNotification(action.Notification);
                await UniTask.Delay((int)(action.waitTime * 1000));
                break;
            case StoryAction.CameraSetup:
                await CameraManager.Instance.MoveAndLook(
                    action.cameraTargetPosition,
                    action.cameraLookOffset,
                    action.cameraMoveDuration,
                    action.cameraLookDuration
                );
                break;
            case StoryAction.MoveObjectGobal:

                if (action.targetID == "Player")
                {
                    if (objectMap.TryGetValue(action.targetID, out var go5))
                    {
                        StartCoroutine(Teleport(go5.transform, action.position));
                        return;
                    }
                }
                if (objectMap.TryGetValue(action.targetID, out var go4))
                {
                    go4.transform.position = action.position;
                    go4.transform.rotation = Quaternion.Euler(action.rotation);
                    storyState.movedObjects.Add(new MovedObjectData
                    {
                        objectID = action.targetID,
                        position = action.position,
                        rotation = action.rotation
                    });
                }
                break;
            case StoryAction.playCutscene:
                if (action.cutSceneAsset != null)
                {
                    SceneTransitionManager.Instance.fadeCanvas.gameObject.SetActive(false);
                    playableDirector.playableAsset = action.cutSceneAsset;
                    playableDirector.Play();
                    await UniTask.WaitUntil(() => playableDirector.state == PlayState.Paused);
                    SceneTransitionManager.Instance.fadeCanvas.gameObject.SetActive(true);
                }
                break;
            case StoryAction.loadScene:
                LoadingScene.Instance.StartLoading(action.sceneName);
                break;
            case StoryAction.minusCoins:
                PlayerData.instance.RemoveCoin(action.minusCoins);
                break;
        }
    }


    IEnumerator Teleport(Transform playerRoot, Vector3 targetPos)
    {
        yield return null; // đợi qua 1 frame để chắc chắn UI đã sẵn sàng
        // 1) TẮT điều khiển chuyển động/physics để tránh ghi đè
        var move = playerRoot ? playerRoot.GetComponent<PlayerMovement>() : null;
        var cc = playerRoot ? playerRoot.GetComponent<CharacterController>() : null;
        var rb = (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null) ?? (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null);
        var anim = playerRoot ? playerRoot.GetComponentInChildren<Animator>() : null;

        if (move) move.enabled = false;
        if (cc) cc.enabled = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // tạm thời để teleport sạch
        }
        if (anim) anim.applyRootMotion = false;


        if (playerRoot) { playerRoot.position = targetPos; playerRoot.localRotation = Quaternion.identity; }

        Physics.SyncTransforms(); // đồng bộ ngay lập tức
        yield return null;        // đợi qua 1 frame, tránh script khác ghi đè trong cùng frame

        // 3) BẬT LẠI theo thứ tự
        if (rb) rb.isKinematic = false;
        if (cc) cc.enabled = true;
        if (move) move.enabled = true;
        if (anim) anim.applyRootMotion = true;
    }

    public async Task SaveStoryState()
    {
        Debug.Log("Saving Story State");

        // 1. Đọc dữ liệu cũ
        var state = await NpcData.instance.LoadStoryStateAsync();

        List<MovedObjectData> movedObjectsTemp = new List<MovedObjectData>();

        foreach (var pair in state.movedObjects)
        {
            movedObjectsTemp.Add(new MovedObjectData
            {
                objectID = pair.objectID,
                position = new Vector3(pair.positionX, pair.positionY, pair.positionZ),
            });
        }

        StoryStateData storyStateTemp = new StoryStateData()
        {
            disabledObjects = state.disabledObjects,
            enabledObjects = state.enabledObjects,
            movedObjects = movedObjectsTemp
        };

        StoryStateData oldState = storyStateTemp;

        // 2. Merge disabledObjects
        foreach (var obj in storyState.disabledObjects)
        {
            if (!oldState.disabledObjects.Contains(obj))
                oldState.disabledObjects.Add(obj);
        }

        // 3. Merge enabledObjects
        foreach (var obj in storyState.enabledObjects)
        {
            if (!oldState.enabledObjects.Contains(obj))
                oldState.enabledObjects.Add(obj);
        }

        // 4. Merge movedObjects (cập nhật nếu trùng ID, thêm nếu mới)
        foreach (var moved in storyState.movedObjects)
        {
            var existing = oldState.movedObjects.Find(m => m.objectID == moved.objectID);
            if (existing != null)
            {
                existing.position = moved.position;
                existing.rotation = moved.rotation;
            }
            else
            {
                oldState.movedObjects.Add(moved);
            }
        }

        List<StoryMovedObject> movedObjects = new List<StoryMovedObject>();
        foreach (var moved in oldState.movedObjects)
        {
            movedObjects.Add(new StoryMovedObject
            {
                objectID = moved.objectID,
                positionX = moved.position.x,
                positionY = moved.position.y,
                positionZ = moved.position.z,
            });
        }

        StoryStateResponse storyStateResponse = new StoryStateResponse
        {
            disabledObjects = oldState.disabledObjects,
            enabledObjects = oldState.enabledObjects,
            movedObjects = movedObjects
        };
         await NpcData.instance.SaveStoryStateAsync(storyStateResponse);

    }

}

