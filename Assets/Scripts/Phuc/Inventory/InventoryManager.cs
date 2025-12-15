using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<GameObject> inventorySlots;
    public GameObject weaponInventorySlot;
    public GameObject inventoryUI;

    [Header("Prefabs")]
    public GameObject inventoryItemPrefab;

    public WeaponManager weaponManager;

    public static InventoryManager instance;

    [System.Serializable]
    public class InventoryItemEntry
    {
        public GameObject slot;
        public WeaponData weaponData;
        public ItemData itemData;
    }

    public List<InventoryItemEntry> currentInventory = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryUI.activeSelf)
            {
                inventoryUI.SetActive(false);
            }
            else
            {
                inventoryUI.SetActive(true);
            }
        }
    }

    public void AddItem(WeaponData weaponData)
    {
        foreach (GameObject slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject itemObj = Instantiate(inventoryItemPrefab, slot.transform);
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.image.sprite = weaponData.icon;
                item.weaponData = weaponData;

                currentInventory.Add(new InventoryItemEntry
                {
                    slot = slot,
                    weaponData = weaponData,
                    itemData = null
                });

                return;
            }
        }

        Debug.LogWarning("Inventory full! Cannot add weapon: " + weaponData.name);
    }
    public void AddItem(ItemData itemData)
    {
        foreach (GameObject slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject itemObj = Instantiate(inventoryItemPrefab, slot.transform);

                // Gán hình ảnh và dữ liệu
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.image.sprite = itemData.icon;
                item.itemData = itemData;

                // Gán controller UI nếu có
                ItemUIController itemUI = itemObj.GetComponent<ItemUIController>();
                if (itemUI != null)
                {
                    itemUI.SetItem(itemData);
                    itemUI.inventoryManager = this;
                }

                currentInventory.Add(new InventoryItemEntry
                {
                    slot = slot,
                    weaponData = null,
                    itemData = itemData
                });

                return;
            }
        }

        Debug.LogWarning("Inventory full! Cannot add item: " + itemData.itemName);
    }

    public void Remove(ItemData itemData, GameObject selfUI)
    {
        for (int i = 0; i < currentInventory.Count; i++)
        {
            InventoryItemEntry entry = currentInventory[i];

            if (entry.itemData == itemData && entry.slot.transform.childCount > 0)
            {
                Transform child = entry.slot.transform.GetChild(0);

                // So sánh đúng object đang muốn xóa
                if (child.gameObject == selfUI)
                {
                    Destroy(child.gameObject);
                    currentInventory.RemoveAt(i);
                    Debug.Log($"[Remove] Removed item: {itemData.itemName}");
                    return;
                }
            }
        }

        Debug.LogWarning($"[Remove] Item not found or mismatch: {itemData.itemName}");
    }
    public void UpdateSlot(InventoryItem movedItem, GameObject newSlot)
    {
        foreach (var entry in currentInventory)
        {
            if (entry.weaponData != null && entry.weaponData == movedItem.weaponData)
            {
                entry.slot = newSlot;
                return;
            }
            if (entry.itemData != null && entry.itemData == movedItem.itemData)
            {
                entry.slot = newSlot;
                return;
            }
        }
        Debug.LogWarning("[UpdateSlot] Không tìm thấy item để cập nhật slot mới!");
    }


}
