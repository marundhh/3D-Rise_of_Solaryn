using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public WeaponManager weaponManager;
    public WeaponData weaponData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            inventoryManager.AddItem(weaponData);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            weaponManager.ChangeWeapon(weaponData);
        }
    }
}
