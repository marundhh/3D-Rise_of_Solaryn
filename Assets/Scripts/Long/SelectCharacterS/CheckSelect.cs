using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSelect : MonoBehaviour
{
    public GameObject WeaponOnDirt;
    public GameObject WeaponOnHand;

    public LayerMask characterMask;
    public Animator animator;
    public SpecCameraSelection specCameraSelection;

    public enum CharacterClass
    {
        Knight,
        Mage,
        Archer
    }

    public CharacterClass characterClass = CharacterClass.Knight;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 2f);
            if (Physics.Raycast(ray, out hit, 1000f, characterMask))
            {
                GameObject selectedObject = hit.collider.gameObject;
                specCameraSelection.ZomeInCharacter(selectedObject);
                if (selectedObject == this.gameObject)
                {
                    ChangeClass();
                    animator.SetBool("OnSelect", true);
                    WeaponOnDirt.SetActive(false);
                    WeaponOnHand.SetActive(true);
                    Debug.Log("chọn");
                } else
                {

                    animator.SetBool("OnSelect", false);
                    WeaponOnDirt.SetActive(true);
                    WeaponOnHand.SetActive(false);
                    Debug.Log("Bỏ chọn");
                }

            }

        }

    }

    private void ChangeClass()
    {
        switch (characterClass)
        {
            case CharacterClass.Knight:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Knight);
                break;
            case CharacterClass.Archer:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Archer);
                break;
            case CharacterClass.Mage:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Mage);
                break;
            default:
                break;
        }
    }
}