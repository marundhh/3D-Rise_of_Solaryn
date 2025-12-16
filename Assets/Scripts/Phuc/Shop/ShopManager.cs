using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Transform shopContent;
    public GameObject shopItemPrefab;

    public List<WeaponData> allWeaponData;
    public List<ItemData> allItemData;

    void Start()
    {
        PopulateShop();
    }

    public void PopulateShop()
    {
        foreach (var weapon in allWeaponData)
        {
            GameObject go = Instantiate(shopItemPrefab, shopContent);
            go.GetComponent<ShopItemEntry>().Setup(weapon);
        }

        foreach (var item in allItemData)
        {
            GameObject go = Instantiate(shopItemPrefab, shopContent);
            go.GetComponent<ShopItemEntry>().Setup(item);
        }
    }

    public void Test()
    {
        Debug.Log("Test");
    }
}

