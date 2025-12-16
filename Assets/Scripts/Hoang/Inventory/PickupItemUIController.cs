using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupItemUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;

    // giữ raw data để phân biệt khi consume
    private ItemData itemData;
    private WeaponData weaponData;
    private int quantity = 1;

    public void Setup(ItemData data, int qty)
    {
        itemData = data;
        weaponData = null;
        quantity = qty;

        if (iconImage != null)
            iconImage.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.itemName;

        UpdateQuantityUI();
    }

    public void Setup(WeaponData data)
    {
        weaponData = data;
        itemData = null;
        quantity = 1;

        if (iconImage != null)
            iconImage.sprite = data.icon; // giả sử tên trường
        if (nameText != null)
            nameText.text = data.weaponName;

        UpdateQuantityUI();
    }

    public void Increment(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityUI();
    }

    void UpdateQuantityUI()
    {
        if (quantityText != null)
        {
            if (itemData != null)
                quantityText.text = quantity > 1 ? $"x{quantity}" : "";
            else
                quantityText.text = ""; // weapon không show số lượng
        }
    }

    public bool IsItem() => itemData != null;
    public bool IsWeapon() => weaponData != null;
    public ItemData GetItemData() => itemData;
    public WeaponData GetWeaponData() => weaponData;
    public int GetQuantity() => quantity;
}
