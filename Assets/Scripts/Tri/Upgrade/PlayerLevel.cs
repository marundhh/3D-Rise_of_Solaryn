using UnityEngine;
using System.Collections.Generic;

public class PlayerLevel : MonoBehaviour
{
    public static PlayerLevel instance;

    public int level = 1;
    public int exp = 0;
    public int expToNext = 100;

    private Queue<int> pendingLevelUps = new Queue<int>();
    private bool isUpgradeUIShowing = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (PlayerData.instance != null)
        {
            if (PlayerData.instance.isNewGame)
            {
                level = 1;
                exp = 0;
                expToNext = 100;

                SyncToPlayerData();
               // PlayerData.instance.isNewGame = false;

                Debug.Log("[PlayerLevel.Awake] New Game → reset level = 1, exp = 0");
            }
            else
            {
                SyncFromPlayerData();
                Debug.Log("[PlayerLevel.Awake] Continue → sync từ PlayerData: level = " + level + ", exp = " + exp);
            }
        }
    }

    private void Start()
    {
        // luôn sync lại để chắc chắn có dữ liệu từ API
        SyncFromPlayerData();
        Debug.Log("[PlayerLevel.Start] Sync lại từ PlayerData: level = " + level + ", exp = " + exp);
    }

    public void GainExp(int amount)
    {
        Debug.Log("[PlayerLevel.GainExp] Nhận EXP = " + amount);

        exp += amount;

        while (exp >= expToNext)
        {
            exp -= expToNext;
            LevelUp();
            pendingLevelUps.Enqueue(level);
        }

        SyncToPlayerData();
        TryShowNextUpgrade();
        Debug.Log("[PlayerLevel.GainExp] Sau khi cộng exp → level = " + level + ", exp = " + exp);
    }

    void LevelUp()
    {
        level++;
        expToNext += 50;
        SyncToPlayerData();

        Debug.Log("[PlayerLevel.LevelUp] Lên cấp! level = " + level + ", expToNext = " + expToNext);
    }

    public void SyncFromPlayerData()
    {
        if (PlayerData.instance != null)
        {
            level = PlayerData.instance.level > 0 ? PlayerData.instance.level : 1;
            exp = PlayerData.instance.experience;

            expToNext = 100 + (level - 1) * 50;

            Debug.Log("[PlayerLevel.SyncFromPlayerData] level = " + level + ", exp = " + exp);
        }
    }

    public void SyncToPlayerData()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.level = level;
            PlayerData.instance.experience = exp;
            Debug.Log("[PlayerLevel.SyncToPlayerData] Ghi sang PlayerData: level = " + level + ", exp = " + exp);
        }
    }

    // ========== UI xử lý nâng cấp ==========
    private void TryShowNextUpgrade()
    {
        if (isUpgradeUIShowing || pendingLevelUps.Count == 0)
            return;

        isUpgradeUIShowing = true;

        UpgradeUI.Instance.Show(() =>
        {
            OnUpgradeUIClosed();
        });
    }

    private void OnUpgradeUIClosed()
    {
        pendingLevelUps.Dequeue();
        isUpgradeUIShowing = false;

        if (pendingLevelUps.Count > 0)
        {
            TryShowNextUpgrade();
        }
    }
}