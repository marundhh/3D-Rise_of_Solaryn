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

        List<UpgradeOption> result = new List<UpgradeOption>();

        // clone và random rarity + value
        foreach (var upgrade in vipList.Concat(normalList))
        {
            result.Add(CreateUpgrade(upgrade));
        }


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

    private UpgradeOption CreateUpgrade(UpgradeOption baseOption)
    {
        UpgradeOption option = new UpgradeOption
        {
            name = baseOption.name,
            icon = baseOption.icon,
            type = baseOption.type,
            isVIP = baseOption.isVIP
        };

        option.rarity = GetRandomRarity();

        option.value = GetRandomValueByRarity(option.rarity, baseOption.value);
        option.description = $" + {option.value}%";

        return option;
    }

    private int GetRandomValueByRarity(UpgradeRarity rarity, int baseValue)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common:
                return UnityEngine.Random.Range(
                    Mathf.RoundToInt(baseValue * 0.8f),
                    Mathf.RoundToInt(baseValue * 1.2f) + 1
                );

            case UpgradeRarity.Rare:
                return UnityEngine.Random.Range(
                    Mathf.RoundToInt(baseValue * 1.2f),
                    Mathf.RoundToInt(baseValue * 1.6f) + 1
                );

            case UpgradeRarity.Epic:
                return UnityEngine.Random.Range(
                    Mathf.RoundToInt(baseValue * 1.6f),
                    Mathf.RoundToInt(baseValue * 2.2f) + 1
                );
        }
        return baseValue;
    }

    public void ApplyUpgrade(UpgradeOption upgrade)
    {
        Debug.Log("ApplyUpgrade được gọi");
        var player = PlayerStats.instance;
        if (player == null) return;

        switch (upgrade.type)
        {
            case UpgradeType.MaxHP:
                player.AddHealthPercent(upgrade.value);
                break;
            case UpgradeType.Armor:
                player.AddArmorPercent(upgrade.value);
                break;
            case UpgradeType.Mana:
                player.AddManaPercent(upgrade.value);
                break;
            case UpgradeType.PhysicalDamage:
                player.AddPhysicalDamagePercent(upgrade.value);
                break;
            case UpgradeType.MagicDamage:
                player.AddMagicDamagePercent(upgrade.value);
                break;
            case UpgradeType.CooldownReduction:
                player.AddCooldownReductionPercent(upgrade.value);
                break;
            case UpgradeType.MoveSpeed:
                player.AddMoveSpeedPercent(upgrade.value);
                break;
            case UpgradeType.AttackSpeed:
                player.AddAttackSpeedPercent(upgrade.value);
                break;
            case UpgradeType.AttackRange:
                player.AddAttackRangePercent(upgrade.value);
                break;
        }
        Debug.Log($"Đã áp dụng nâng cấp: {upgrade.name} (+{upgrade.value} {upgrade.type})");
    }
}
