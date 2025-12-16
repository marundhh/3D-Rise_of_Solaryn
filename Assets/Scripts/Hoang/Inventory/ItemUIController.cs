using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemUIController : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    public InventoryManager inventoryManager;

    public TextMeshProUGUI quantityText;


    public void UpdateQuantity(int quantity)
    {
        if (quantityText != null)
        {
            // Chỉ hiển thị số lượng nếu là item thường (không phải vũ khí)
            if (itemData != null)
            {
                if (quantity > 1)
                {
                    quantityText.gameObject.SetActive(true);
                    quantityText.text = quantity.ToString();
                }
                else
                {
                    quantityText.gameObject.SetActive(false);
                }
            }
            
        }
    }


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

        // Tìm entry tương ứng
        foreach (var entry in inventoryManager.currentInventory)
        {
            if (entry.itemData == itemData && entry.slot == this.transform.parent.gameObject)
            {
                // Dùng item
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
                }

               
                if (itemData.isStackable)
                {
                    entry.quantity--;

                    if (entry.quantity <= 0)
                    {
                        inventoryManager.Remove(itemData, this.gameObject);
                    }
                    else
                    {
                        UpdateQuantity(entry.quantity);
                    }
                }
                else
                {
                    inventoryManager.Remove(itemData, this.gameObject);
                }

                return;
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        UseItem(eventData);
    }

}
