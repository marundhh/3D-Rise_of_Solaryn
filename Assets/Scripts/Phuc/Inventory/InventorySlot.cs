using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool isWeaponSlot;
    public InventoryManager inventoryManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0)
        {
            Debug.LogWarning("[OnDrop] Slot already has an item. Cannot overwrite.");
            return;
        }

        InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        inventoryItem.transform.SetParent(transform);
        inventoryItem.parentAfterDrag = transform;

        inventoryManager.UpdateSlot(inventoryItem, transform.gameObject);

        if (isWeaponSlot && inventoryManager != null)
        {
            inventoryManager.weaponManager.ChangeWeapon(inventoryItem.weaponData);
        }
    }
}
