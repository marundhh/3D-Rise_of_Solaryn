using UnityEngine;

[System.Serializable]
public class InventoryItemEntry
{
    public GameObject slot;
    public WeaponData weaponData;
    public ItemData itemData;
    public int quantity; // Số lượng item nếu là stackable
}
