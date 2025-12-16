using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance { get; private set; }

    [Header("Soul Settings")]
    public int maxStoredSouls = 10;

    private List<EnemySoulData> storedSouls = new List<EnemySoulData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject); // Optional
    }

    public void StoreSoul(EnemyStats enemy)
    {
        if (enemy == null) return;

        EnemySoulData newSoul = new EnemySoulData(enemy.MaxHealth, enemy.damage);
        storedSouls.Add(newSoul);

        if (storedSouls.Count > maxStoredSouls)
        {
            storedSouls.RemoveAt(0); // Xoá linh hồn cũ nhất
        }
    }

    public EnemySoulData PopLastSoul()
    {
        if (storedSouls.Count == 0) return null;

        var soul = storedSouls[storedSouls.Count - 1];
        storedSouls.RemoveAt(storedSouls.Count - 1); // Xóa khỏi danh sách
        return soul;
    }

    public EnemySoulData GetRandomSoul()
    {
        if (storedSouls.Count == 0) return null;
        int index = Random.Range(0, storedSouls.Count);
        return storedSouls[index];
    }

    public EnemySoulData GetLastSoul()
    {
        if (storedSouls.Count == 0) return null;
        return storedSouls[storedSouls.Count - 1];
    }

    public void ClearSouls()
    {
        storedSouls.Clear();
    }

    public int GetSoulCount()
    {
        return storedSouls.Count;
    }
}
