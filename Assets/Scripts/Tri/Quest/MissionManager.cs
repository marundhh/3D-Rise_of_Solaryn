using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;
// ❌ KHÔNG dùng UnityEngine.EventSystems ở đây

public class MissionManager : MonoBehaviour
{
    public Transform playerTransform;

    public static MissionManager Instance { get; private set; }

    [SerializeField] private List<Mission> activeMissions = new List<Mission>();
    private Dictionary<int, Mission> missionTemplates = new Dictionary<int, Mission>();

    public GameObject markerPrefab;
    public GameObject minimapIconPrefab;

    private void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        GameEventSystem.OnGameEvent += OnGameEvent;
    }

    private void OnDisable()
    {
        GameEventSystem.OnGameEvent -= OnGameEvent;
    }

    public void AddMission(Mission mission)
    {
        activeMissions.Add(mission);

        Transform target = mission.GetTargetTransform();
        if (target != null)
        {
            Debug.Log($"🟢 Found target: {target.name}");

            if (markerPrefab != null)
            {
                GameObject marker = Instantiate(markerPrefab, target.position, Quaternion.identity);
                marker.transform.SetParent(target);
                Debug.Log($"✨ Marker created on {target.name}");
            }
            else Debug.LogWarning("❌ Marker prefab is null!");
            if (minimapIconPrefab != null && playerTransform != null)
            {
                GameObject icon = Instantiate(minimapIconPrefab, target.position + Vector3.up * 10f, Quaternion.identity);
                icon.transform.SetParent(target);

                var follow = icon.GetComponent<MissionMinimapIcon>();
                follow.Init(target, playerTransform);
                Debug.Log($"📍 Minimap icon created on {target.name}");
            }

        }
        else
        {
            Debug.LogWarning("❌ Target transform is null! Cannot show marker.");
        }
    }


    private void OnGameEvent(GameEvent e)
    {
        foreach (var mission in activeMissions)
        {
            if (!mission.IsCompleted)
                mission.OnEvent(e);
        }
    }

    public List<Mission> GetAllMissions() => activeMissions;
    public void ClearAllMions()
    {
        activeMissions.Clear();
        missionTemplates.Clear();
        Debug.Log("All missions cleared.");
    }
    public Mission GetMissionByID(int id)
    {
        return activeMissions.Find(a => a.missionID == id);
    }

    // -------------------- MỞ RỘNG CHO DATABASE -----------------------

    public void AcceptNewMission(MissionType missionType, int id, string tile, string decription, string type, int count, string loacation, int rewardGold, int rewardExp, string storyID, bool isShow = false, int currentCount = 0)
    {
        Debug.Log("In create mision");
        if (missionTemplates.ContainsKey(id)) return;

        Debug.Log("Start create mision");
        if (!isShow)
        {
            GetComponent<MissionDisplay>().ShowMissionDetails(new MissionRaw
            {
                id = id,
                tile = tile,
                description = decription,
                type = type,
                count = count,
                loacation = loacation,
                rewardGold = rewardGold,
                rewardExp = rewardExp,
                missionType = missionType
            });
        }

        switch (missionType)
        {
            case MissionType.FindWay:
                Transform targetPoint = StoryExecutor.Instance.GetTransfromByID(loacation);
                missionTemplates.Add(id, new ReachLocationMission(id, tile, decription, type, targetPoint, rewardGold, rewardExp, storyID));
                AcceptMissionByID(id);
                break;

            case MissionType.TalkToNPC:
                Transform targetPoint2 = StoryExecutor.Instance.GetTransfromByID(loacation);
                missionTemplates.Add(id, new TalkToNPCMission(id, tile, decription, type, targetPoint2, rewardGold, rewardExp));
                AcceptMissionByID(id);
                break;

            case MissionType.KillEnemy:
                Transform targetPoint3 = StoryExecutor.Instance.GetTransfromByID(loacation);
                missionTemplates.Add(id, new KillEnemyMission(id, tile, decription, type, count, targetPoint3, rewardGold, rewardExp, storyID, currentCount));
                AcceptMissionByID(id);
                break;
            case MissionType.CollectionItem:
                Transform targetPoint4 = StoryExecutor.Instance.GetTransfromByID(loacation);
                missionTemplates.Add(id, new CollectItemMission(id, tile, decription, type, count, targetPoint4, rewardGold, rewardExp, storyID, currentCount));
                AcceptMissionByID(id);
                break;
            default:
                Debug.Log("Không tạo nhiệm vụ không có loại nhiệm vụ như này" + missionType);
                break;
        }
    }

    public void AcceptMissionByID(int id)
    {
        if (missionTemplates.TryGetValue(id, out Mission template))
        {
            Mission cloned = CloneMission(template);
            if (cloned != null)
                AddMission(cloned); // 👈 Hiện marker nằm trong AddMission()
                                    //   Debug.Log("Da nhan nhiem vu");
        }
        else
        {
            Debug.LogWarning($"Mission ID {id} không tồn tại.");
        }
    }

    private Mission CloneMission(Mission m)
    {
        if (m is KillEnemyMission km)
            return new KillEnemyMission(km.missionID, km.Title, km.Description, km.EnemyType, km.RequiredKillCount, km.targetPoint, km.GetGoldReward(), km.GetExpReward(), km.GetStoryID(), km.CurrentKillCount);
        else if (m is TalkToNPCMission tm)
            return new TalkToNPCMission(tm.missionID, tm.Title, tm.Description, tm.NPCName, tm.targetPoint, tm.GetGoldReward(), tm.GetExpReward());
        else if (m is ReachLocationMission rm)
            return new ReachLocationMission(rm.missionID, rm.Title, rm.Description, rm.LocationName, rm.targetPoint, rm.GetGoldReward(), rm.GetExpReward(), rm.GetStoryID());
        else if (m is CollectItemMission cm)
            return new CollectItemMission(cm.missionID, cm.Title, cm.Description, cm.ItemType, cm.RequiredCount, cm.targetPoint, cm.GetGoldReward(), cm.GetExpReward(), cm.GetStoryID(), cm.CurrentCount);
        else
            return null;
    }

    public List<MissionSaveData> GetMissionSaveList()
    {
        List<MissionSaveData> list = new List<MissionSaveData>();
        foreach (var mission in activeMissions)
        {
            MissionSaveData data = new MissionSaveData
            {
                missionID = mission.missionID,
                title = mission.Title,
                description = mission.Description,
                missionType = GetMissionType(mission),
                isCompleted = mission.IsCompleted,
                rewardGold = mission.GetGoldReward(),
                rewardExp = mission.GetExpReward(),
            };

            switch (mission)
            {
                case KillEnemyMission km:
                    data.type = km.EnemyType;
                    data.count = km.RequiredKillCount;
                    data.currentCount = km.CurrentKillCount;
                    data.location = km.targetPoint?.name ?? "";
                    data.storyID = km.GetStoryID();
                    break;
                case TalkToNPCMission tm:
                    data.type = tm.NPCName;
                    data.location = tm.targetPoint?.name ?? "";
                    break;
                case ReachLocationMission rm:
                    data.type = rm.LocationName;
                    data.location = rm.targetPoint?.name ?? "";
                    break;
                case CollectItemMission cm:
                    data.type = cm.ItemType;
                    data.count = cm.RequiredCount;
                    data.currentCount = cm.CurrentCount;

                    break;
            }

            list.Add(data);
        }
        return list;
    }

    private MissionType GetMissionType(Mission mission)
    {
        if (mission is KillEnemyMission) return MissionType.KillEnemy;
        if (mission is TalkToNPCMission) return MissionType.TalkToNPC;
        if (mission is ReachLocationMission) return MissionType.FindWay;
        if (mission is CollectItemMission) return MissionType.CollectionItem;
        return MissionType.FindWay; // default
    }


    public async Task SaveMissionsToFile()
    {
        MissionSaveListWrapper oldData = new MissionSaveListWrapper
        {
            missions = await NpcData.instance.LoadMissionsAsync()
        };

        // 2. Chuyển dữ liệu cũ sang dictionary (key có thể là missionId)
        Dictionary<int, MissionSaveData> mergedData = new Dictionary<int, MissionSaveData>();
        foreach (var mission in oldData.missions)
        {
            mergedData[mission.missionID] = mission;
        }

        // 3. Merge từ dữ liệu mới
        var newMissions = GetMissionSaveList();
        foreach (var mission in newMissions)
        {
            mergedData[mission.missionID] = mission; // ghi đè nếu tồn tại, thêm nếu chưa có
        }

        var missions = new List<MissionSaveData>(mergedData.Values);

       await NpcData.instance.SaveMissionsAsync(missions);
    }
    public async Task LoadMissionsFromFile()
    {
        var listMissions = await NpcData.instance.LoadMissionsAsync();

        var wrapper = new MissionSaveListWrapper
        {
            missions = listMissions
        };

        activeMissions.Clear();

        for (int i = 0; i < wrapper.missions.Count; i++)
        {
            var missionData = wrapper.missions[i];
            AcceptNewMission(missionData.missionType, missionData.missionID, missionData.title, missionData.description,
               missionData.type, missionData.count, missionData.location, missionData.rewardGold,
               missionData.rewardExp, missionData.storyID, true, missionData.currentCount);
            Debug.Log($"Đã tải nhiệm vụ: {missionData.title} (ID: {missionData.missionID}) | lacation {missionData.location}| current count {missionData.currentCount}");

            if (missionData.isCompleted)
            {
                Debug.Log($"Nhiệm vụ {missionData.title} đã hoàn thành.");
                var mission = GetMissionByID(missionData.missionID);
                if (mission != null)
                {
                    mission.IsCompleted = true;
                }
            }

        }

    }
    public async Task ClearAllMissions()
    {
        // 1. Xóa dữ liệu trong bộ nhớ
        activeMissions.Clear();
        missionTemplates.Clear();

        // 2. Ghi đè file rỗng
        var emptyWrapper = new List<MissionSaveData>(); // Danh sách rỗng
        await  NpcData.instance.SaveMissionsAsync(emptyWrapper);
        Debug.Log("Đã xóa tất cả nhiệm vụ và ghi đè file rỗng.");
    }
}


[System.Serializable]
public class MissionSaveListWrapper
{
    public List<MissionSaveData> missions;
}

[System.Serializable]
public class MissionSaveData
{
    public int missionID;
    public string title;
    public string description;
    public string type;
    public int count;
    public int currentCount;
    public string location;
    public int rewardGold;
    public int rewardExp;
    [JsonConverter(typeof(StringEnumConverter))]
    public MissionType missionType { get; set; }
    public bool isCompleted;
    public string storyID = string.Empty;
}


public enum MissionType
{
    FindWay,
    KillEnemy,
    TalkToNPC,
    CollectionItem,
}
