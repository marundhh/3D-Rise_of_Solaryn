using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public int value;
    public string description;

    public bool isStackable = true;

    public int price;

    public ItemType itemType;

    public enum ItemType
    {
        Health,
        Mana,
        Buff
    }
}
