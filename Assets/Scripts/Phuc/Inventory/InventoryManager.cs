using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class InventoryManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<GameObject> inventorySlots;
    public GameObject weaponInventorySlot;
    public GameObject inventoryUI;
    public UIEffectController inventoryUI_1;
    public UIEffectController inventoryUI_2;

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

        public int quantity { get; internal set; }
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

    private void Start()
    {
        InitInventoryWhenNewGame();
    }

    private void InitInventoryWhenNewGame()
    {
        if (!PlayerData.instance.isNewGame) return;

        if (weaponInventorySlot.transform.childCount > 0)
        {
            Destroy(weaponInventorySlot.transform.GetChild(0).gameObject);
        }

        GameObject itemObj = Instantiate(inventoryItemPrefab, weaponInventorySlot.transform);
        InventoryItem item = itemObj.GetComponent<InventoryItem>();
        item.image.sprite = ClassDataPlayerChoose.instance.weaponData.icon;
        item.weaponData = ClassDataPlayerChoose.instance.weaponData;

        currentInventory.Add(new InventoryItemEntry
        {
            slot = weaponInventorySlot,
            weaponData = ClassDataPlayerChoose.instance.weaponData,
            itemData = null
        });

        if (weaponManager != null)
        {
            weaponManager.ChangeWeapon(ClassDataPlayerChoose.instance.weaponData);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (!inventoryUI.activeSelf) inventoryUI.SetActive(true);
            if (inventoryUI_1.enabled && inventoryUI_2.enabled)
            {
                inventoryUI_1.enabled = false;
                inventoryUI_2.enabled = false;
            }
            else
            {
                inventoryUI_1.enabled = true;
                inventoryUI_2.enabled = true;
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
   
        if (itemData.isStackable)
        {
            foreach (var entry in currentInventory)
            {
                if (entry.itemData == itemData && entry.quantity < 99)
                {
                    entry.quantity++;
                    entry.slot.GetComponentInChildren<ItemUIController>()?.UpdateQuantity(entry.quantity);
                    return;
                }
            }
        }

        foreach (GameObject slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                GameObject itemObj = Instantiate(inventoryItemPrefab, slot.transform);

                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.image.sprite = itemData.icon;
                item.itemData = itemData;

                ItemUIController itemUI = itemObj.GetComponent<ItemUIController>();
                if (itemUI != null)
                {
                    itemUI.SetItem(itemData);
                    itemUI.inventoryManager = this;
                    itemUI.UpdateQuantity(1);
                }

                currentInventory.Add(new InventoryItemEntry
                {
                    slot = slot,
                    weaponData = null,
                    itemData = itemData,
                    quantity = 1
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
