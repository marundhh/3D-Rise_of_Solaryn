using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public ItemData itemData;
    public WeaponData weaponData;

    [HideInInspector] public Transform parentAfterDrag;

    private bool isPointerOver = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {

            string description = $"{itemData.description}\nPrice: {itemData.price}";
            TooltipSystem.Instance.Show(itemData.itemName, description);
        }
        else if (weaponData != null)
        {
            string description = $"Damage: {weaponData.damage}\nPrice: {weaponData.price}";
            TooltipSystem.Instance.Show(weaponData.weaponName, description);
        }
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

    private void OnDestroy()
    {
        if (TooltipSystem.Instance != null)
        {
            TooltipSystem.Instance.Hide();
        }
    }
}
