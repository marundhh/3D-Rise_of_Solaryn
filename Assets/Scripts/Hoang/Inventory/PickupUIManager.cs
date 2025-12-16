using System.Collections.Generic;
using UnityEngine;

public class PickupUIManager : MonoBehaviour
{
    public static PickupUIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject pickupUIPrefab; // prefab có PickupItemUIController
    [SerializeField] private Transform pickupPanel;

    // key: riêng cho item (gộp theo reference), weapon giữ riêng từng cái
    private Dictionary<ItemData, PickupItemUIController> visibleItems = new();
    private List<PickupItemUIController> visibleWeapons = new();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Item
    public void RegisterVisible(ItemData data, int amount)
    {
        if (data == null || pickupUIPrefab == null || pickupPanel == null) return;

        if (visibleItems.TryGetValue(data, out var ui))
        {
            ui.Increment(amount);
        }
        else
        {
            GameObject go = Instantiate(pickupUIPrefab, pickupPanel);
            var controller = go.GetComponent<PickupItemUIController>();
            controller.Setup(data, amount);
            visibleItems[data] = controller;
        }
    }

    public void UnregisterVisible(ItemData data)
    {
        if (data == null) return;
        if (visibleItems.TryGetValue(data, out var ui))
        {
            Destroy(ui.gameObject);
            visibleItems.Remove(data);
        }
    }

    public void Consume(ItemData data)
    {
        UnregisterVisible(data);
    }

    // Weapon (không gộp)
    public void RegisterVisible(WeaponData data)
    {
        if (data == null || pickupUIPrefab == null || pickupPanel == null) return;

        GameObject go = Instantiate(pickupUIPrefab, pickupPanel);
        var controller = go.GetComponent<PickupItemUIController>();
        controller.Setup(data);
        visibleWeapons.Add(controller);
    }

    public void UnregisterVisible(WeaponData data)
    {
        if (data == null) return;
        // tìm controller tương ứng và bỏ
        for (int i = visibleWeapons.Count - 1; i >= 0; i--)
        {
            var ctrl = visibleWeapons[i];
            if (ctrl != null && ctrl.IsWeapon() && ctrl.GetWeaponData() == data)
            {
                Destroy(ctrl.gameObject);
                visibleWeapons.RemoveAt(i);
                break;
            }
        }
    }

    public void Consume(WeaponData data)
    {
        UnregisterVisible(data);
    }

    public void ClearAll()
    {
        foreach (var kv in visibleItems)
        {
            if (kv.Value != null) Destroy(kv.Value.gameObject);
        }
        visibleItems.Clear();

        foreach (var w in visibleWeapons)
        {
            if (w != null) Destroy(w.gameObject);
        }
        visibleWeapons.Clear();
    }
}
