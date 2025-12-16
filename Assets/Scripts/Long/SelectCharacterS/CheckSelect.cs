using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheckSelect : MonoBehaviour
{
    public GameObject WeaponOnDirt;
    public GameObject WeaponOnHand;

    public LayerMask characterMask;
    public Animator animator;
    public SpecCameraSelection specCameraSelection;
    public CharacterClassData CharacterClassData;


    //Show info class
    public GameObject infoPanel;
    public TextMeshProUGUI classNameText;
    public TextMeshProUGUI classDescriptionText;
    public TextMeshProUGUI skill1Text;
    public TextMeshProUGUI skill2Text;
    public TextMeshProUGUI skill3Text;
    public Image skill1Img;
    public Image skill2Img;
    public Image skill3Img;
    public Sprite skill1Spr;
    public Sprite skill2Spr;
    public Sprite skill3Spr;

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
                    ShowClassInfo();
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UnClassInfo();
            specCameraSelection.ZomeOutCharacter();
            animator.SetBool("OnSelect", false);
            WeaponOnDirt.SetActive(true);
            WeaponOnHand.SetActive(false);
            Debug.Log("Bỏ chọn");
        }

    }

    public void ShowClassInfo()
    {
        infoPanel.SetActive(true);
        switch (characterClass)
        {
            case CharacterClass.Knight:
                classNameText.text = "The Knight";
                classDescriptionText.text = "A young warrior in a remote village, he has a great desire, a strong will, he will defeat the tower.";
                skill1Text.text = "Spin and swing your sword, dealing continuous damage.";
                skill2Text.text = "Create a protective layer for yourself.";
                skill3Text.text = "Weapon enhancement greatly increases damage.";
                break;
            case CharacterClass.Archer:
                classNameText.text = "The Archer";
                classDescriptionText.text = "A wandering archer from the ancient holy land, who has trained to the point that each arrow he shoots carries a sacred light.";
                skill1Text.text = "Fires 3 arrows that deal cone damage.";
                skill2Text.text = "Increase attack speed and reduce damage taken.";
                skill3Text.text = "Creates a rain of arrows that deal continuous area damage.";
                break;
            case CharacterClass.Mage:
                classNameText.text = "The Mage";
                classDescriptionText.text = "A mage who was expelled from the magic council for researching forbidden magic.";
                skill1Text.text = "Summon 3 skeleton warriors.";
                skill2Text.text = "Heals summoned monsters.";
                skill3Text.text = "Buff damage to summoned monsters.";
                break;
            default:
                classNameText.text = "";
                classDescriptionText.text = "";
                break;
        }
        skill1Img.sprite = skill1Spr;
        skill2Img.sprite = skill2Spr;
        skill3Img.sprite = skill3Spr;
    }
    public void UnClassInfo()
    {
        classNameText.text = "";
        classDescriptionText.text = "";
        infoPanel.SetActive(false);
    }

    private void ChangeClass()
    {
        switch (characterClass)
        {
            case CharacterClass.Knight:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Knight);
                PlayerData.instance.classId = 1;
                break;
            case CharacterClass.Archer:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Archer);
                PlayerData.instance.classId = 2;
                break;
            case CharacterClass.Mage:
                ClassDataPlayerChoose.instance.ChangeClass(ClassDataPlayerChoose.CharacterClass.Mage);
                PlayerData.instance.classId = 2;
                break;
            default:
                break;
        }
    }
}