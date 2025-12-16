using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemEntry : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    private WeaponData weaponData;
    private ItemData itemData;

    public void Setup(WeaponData weapon)
    {
        weaponData = weapon;
        icon.sprite = weapon.icon;
        nameText.text = weapon.name;
        priceText.text = weapon.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            if (PlayerData.instance.RemoveCoin(weapon.price))
            {
                InventoryManager.instance.AddItem(weaponData);
            }
        });
    }

    public void Setup(ItemData item)
    {
        Debug.Log("Set Up");
        itemData = item;
        icon.sprite = item.icon;
        nameText.text = item.itemName;
        priceText.text = item.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() =>
        {
            if (PlayerData.instance.RemoveCoin(item.price))
            {
                InventoryManager.instance.AddItem(itemData);
            }
        });
    }
}

