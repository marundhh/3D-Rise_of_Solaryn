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
            TooltipSystem.Instance.Show(itemData.itemName, itemData.description);
        }
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide(); // <-- lỗi chỗ này cũng dùng Show
    }

}
