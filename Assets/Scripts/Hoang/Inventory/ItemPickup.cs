using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Pickup Content")]
    public ItemData itemData;           // có thể null nếu không phải item
    [Tooltip("Số lượng nếu stackable (chỉ áp dụng cho item)")]
    public int itemAmount = 1;

    public WeaponData weaponData;       // có thể null nếu không phải weapon

    [Header("Input")]
    public KeyCode pickupKey = KeyCode.X;

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(pickupKey))
        {
            PickupAll();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = true;

        if (itemData != null)
        {
            PickupUIManager.Instance.RegisterVisible(itemData, itemAmount);
        }

        if (weaponData != null)
        {
            PickupUIManager.Instance.RegisterVisible(weaponData);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;

        if (itemData != null)
        {
            PickupUIManager.Instance.UnregisterVisible(itemData);
        }

        if (weaponData != null)
        {
            PickupUIManager.Instance.UnregisterVisible(weaponData);
        }
    }

    private void PickupAll()
    {
        if (itemData != null)
        {
            if (InventoryManager.instance != null)
                InventoryManager.instance.AddItem(itemData);

            PickupUIManager.Instance.Consume(itemData);
        }

        if (weaponData != null)
        {
            if (InventoryManager.instance != null)
                InventoryManager.instance.AddItem(weaponData);

            PickupUIManager.Instance.Consume(weaponData);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // phòng trường hợp object bị hủy mà UI chưa dọn
        if (itemData != null)
            PickupUIManager.Instance.UnregisterVisible(itemData);
        if (weaponData != null)
            PickupUIManager.Instance.UnregisterVisible(weaponData);
    }
}
