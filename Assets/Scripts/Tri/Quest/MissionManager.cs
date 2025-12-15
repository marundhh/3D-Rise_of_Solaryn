using System;
using System.Collections.Generic;
using UnityEngine;
// ❌ KHÔNG dùng UnityEngine.EventSystems ở đây

public class MissionManager : MonoBehaviour
{
    public Transform playerTransform;

    public static MissionManager Instance { get; private set; }

    [SerializeField] private List<Mission> activeMissions = new List<Mission>();
    private Dictionary<int, Mission> missionTemplates = new Dictionary<int, Mission>();

    // 👇 GÁN PREFAB CỦA MARKER & MINIMAP ICON TRONG INSPECTOR
    public GameObject markerPrefab;
    public GameObject minimapIconPrefab;

    private void Awake()
    {
        Instance = this;
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

    public Mission GetMissionByID(int id)
    {
        return activeMissions.Find(a => a.missionID == id);
    }

    // -------------------- MỞ RỘNG CHO DATABASE -----------------------

    public void AcceptNewMission(MissionType missionType, int id, string tile, string decription, string type, int count, string loacation, int rewardGold, int rewardExp, string storyID, bool isCompleted = false)
    {
        Debug.Log("In create mision");
        if (missionTemplates.ContainsKey(id)) return;

        Debug.Log("Start create mision");
        if(!isCompleted)
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
                missionTemplates.Add(id, new ReachLocationMission(id, tile, decription, type, targetPoint, rewardGold, rewardExp));
                AcceptMissionByID(id);
                break;

            case MissionType.TalkToNPC:
              //  Debug.Log(loacation);
                Transform targetPoint2 = StoryExecutor.Instance.GetTransfromByID(loacation);
             //   Debug.Log(targetPoint2);
                missionTemplates.Add(id, new TalkToNPCMission(id, tile, decription, type, targetPoint2, rewardGold, rewardExp));
                AcceptMissionByID(id);
                break;

            case MissionType.KillEnemy:
                Transform targetPoint3 = StoryExecutor.Instance.GetTransfromByID(loacation);
                missionTemplates.Add(id, new KillEnemyMission(id, tile, decription, type, count, targetPoint3, rewardGold, rewardExp, storyID));
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
            return new KillEnemyMission(km.missionID, km.Title, km.Description, km.EnemyType, km.RequiredKillCount, km.targetPoint, km.GetGoldReward(), km.GetExpReward(), km.GetStoryID());
        else if (m is TalkToNPCMission tm)
            return new TalkToNPCMission(tm.missionID, tm.Title, tm.Description, tm.NPCName, tm.targetPoint, tm.GetGoldReward(), tm.GetExpReward());
        else if (m is ReachLocationMission rm)
            return new ReachLocationMission(rm.missionID, rm.Title, rm.Description, rm.LocationName, rm.targetPoint, rm.GetGoldReward(), rm.GetExpReward());
        else if (m is CollectItemMission cm)
            return new CollectItemMission(cm.missionID, cm.Title, cm.Description, cm.ItemType, cm.RequiredCount);
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
                    break;
            }

            list.Add(data);
        }
        return list;
    }
    public void MarkerCompleteMission(int id)
    {
        GetMissionByID(id).IsCompleted = true;
    }

    private MissionType GetMissionType(Mission mission)
    {
        if (mission is KillEnemyMission) return MissionType.KillEnemy;
        if (mission is TalkToNPCMission) return MissionType.TalkToNPC;
        if (mission is ReachLocationMission) return MissionType.FindWay;
        if (mission is CollectItemMission) return MissionType.CollectionItem;
        return MissionType.FindWay; // default
    }

    public void SaveMissionsToFile()
    {
        var missionDataList = GetMissionSaveList();
        string json = JsonUtility.ToJson(new MissionSaveListWrapper { missions = missionDataList });
        System.IO.File.WriteAllText(Application.persistentDataPath + "/missions.json", json);
    }
    public void LoadMissionsFromFile()
    {
        Debug.Log("Đang tải nhiệm vụ từ file...");
        string path = Application.persistentDataPath + "/missions.json";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning(" Không tìm thấy file mission save.");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        MissionSaveListWrapper wrapper = JsonUtility.FromJson<MissionSaveListWrapper>(json);

       activeMissions.Clear();
      /*          foreach(var data in wrapper.missions)
                {
                    AcceptNewMission(data.missionType, data.missionID, data.title, data.description, data.type, data.count, data.location, data.rewardGold, data.rewardExp, data.storyID);
                    if (data.isCompleted)
                    {
                        Debug.Log($"Nhiệm vụ {data.title} đã hoàn thành.");
                        GetMissionByID(data.missionID).IsCompleted = true; // Đánh dấu nhiệm vụ là đã hoàn thành
                    }
                }*/
        for (int i = 0; i < wrapper.missions.Count; i++)
        {
            if (wrapper.missions[i].missionID == wrapper.missions[wrapper.missions.Count - 1].missionID)
            {
                continue;
            }
            AcceptNewMission(wrapper.missions[i].missionType, wrapper.missions[i].missionID, wrapper.missions[i].title, wrapper.missions[i].description, 
                wrapper.missions[i].type, wrapper.missions[i].count, wrapper.missions[i].location, wrapper.missions[i].rewardGold,
                wrapper.missions[i].rewardExp, wrapper.missions[i].storyID, wrapper.missions[i].isCompleted);
            Debug.Log($"Đã tải nhiệm vụ: {wrapper.missions[i].title} (ID: {wrapper.missions[i].missionID})");
            if (wrapper.missions[i].isCompleted) 
            {
                Debug.Log($"Nhiệm vụ {wrapper.missions[i].title} đã hoàn thành.");
                GetMissionByID(wrapper.missions[i].missionID).IsCompleted = true;
            }

        }

    }
    public void ClearAllMissions()
    {
        activeMissions.Clear();
        missionTemplates.Clear();
        Debug.Log("Đã xóa tất cả nhiệm vụ.");
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
    public string location;
    public int rewardGold;
    public int rewardExp;
    public MissionType missionType;
    public bool isCompleted;
    public string storyID; 
}
public enum MissionType
{
    FindWay,
    KillEnemy,
    TalkToNPC,
    CollectionItem,
}
