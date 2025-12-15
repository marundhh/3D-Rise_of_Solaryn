using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIController : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    public InventoryManager inventoryManager;
    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public void Remove()
    {
        inventoryManager.Remove(itemData, this.gameObject);

        Destroy(this.gameObject);
    }

    public void UseItem(PointerEventData eventData = null)
    {
        if (eventData != null && eventData.button != PointerEventData.InputButton.Right)
            return;

        if (itemData == null)
            return;

        Remove();

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Health:
                PlayerStats.instance.currentHealth += itemData.value;
                break;
            case ItemData.ItemType.Mana:
                PlayerStats.instance.currentMana += itemData.value;
                break;
            case ItemData.ItemType.Buff:
                Debug.Log("[UseItem] Buff item activated!");
                break;
            default:
                Debug.LogWarning("[UseItem] Unknown item type!");
                break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UseItem(eventData); // truyền event để check trong UseItem
    }

}
