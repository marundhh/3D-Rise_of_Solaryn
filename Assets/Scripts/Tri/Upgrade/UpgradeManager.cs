using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeOption> allVIPUpgrades;
    public List<UpgradeOption> allNormalUpgrades;

    void Awake() => Instance = this;

    public List<UpgradeOption> GenerateFixed5Upgrades()
    {
        var vipList = allVIPUpgrades.OrderBy(x => Random.value).Take(2).ToList();
        var normalList = allNormalUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        // Gán độ hiếm (nếu cần)
        foreach (var upgrade in vipList.Concat(normalList))
        {
            upgrade.rarity = GetRandomRarity();
        }

        List<UpgradeOption> result = vipList.Concat(normalList).ToList();
        Shuffle(result);
        return result;
    }

    void Shuffle(List<UpgradeOption> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            int randIndex = Random.Range(i, list.Count);
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    UpgradeRarity GetRandomRarity()
    {
        float roll = Random.value;
        if (roll < 0.6f) return UpgradeRarity.Common;
        else if (roll < 0.9f) return UpgradeRarity.Rare;
        else return UpgradeRarity.Epic;
    }
}
