using UnityEngine;

[System.Serializable]
public class UpgradeOption
{
    public string name;
    public string description;
    public Sprite icon;
    public UpgradeType type;
    public int value;
    public bool isVIP;
    public UpgradeRarity rarity;
}
