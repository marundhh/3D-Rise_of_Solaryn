using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Data/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Properties")]
    public int id;
    public float damage;
    public bool isHand = true;

    public int price;
    public string weaponName;
    

    [Header("Weapon References")]
    public GameObject weaponModel;
    public RuntimeAnimatorController controller;
    public Sprite icon;

    public void Init()
    {
        AnimationManager.instance.animator.runtimeAnimatorController = controller;
    }
}
