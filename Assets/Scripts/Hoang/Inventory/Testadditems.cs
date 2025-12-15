using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testadditems : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemData itemData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            inventoryManager.AddItem(itemData);
        }


    }
}
