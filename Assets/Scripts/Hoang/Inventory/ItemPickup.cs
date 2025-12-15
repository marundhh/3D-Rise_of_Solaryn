using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData itemData;            // ScriptableObject chứa thông tin item
    public WeaponData weaponData;
    public GameObject pickupCanvas = null;   // UI canvas “Press E to pick up”

    private bool isPlayerNearby = false;

    void Start()
    {
        // Nếu canvas không null thì tắt
        if (pickupCanvas != null)
        {
            pickupCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.X))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        if (itemData != null)
        {
            InventoryManager.instance.AddItem(itemData);
        }

        if (weaponData != null)
        {
            InventoryManager.instance.AddItem(weaponData);
        }

        if (pickupCanvas != null)
        {
            pickupCanvas.SetActive(false);
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            if (pickupCanvas != null)
            {
                pickupCanvas.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (pickupCanvas != null)
            {
                pickupCanvas.SetActive(false);
            }
        }
    }
}
