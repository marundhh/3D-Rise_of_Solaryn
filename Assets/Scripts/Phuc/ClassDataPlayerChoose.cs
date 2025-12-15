using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassDataPlayerChoose : MonoBehaviour
{
    public static ClassDataPlayerChoose instance;
    public List<ClassData> classData;
    public CharacterClassData characterClassData;
    public WeaponData weaponData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum CharacterClass
    {
        Knight,
        Mage,
        Archer
    }

    public CharacterClass Class = CharacterClass.Knight;

    public void ChangeClass(CharacterClass c)
    {
        Class = c;
        switch (Class)
        {
            case CharacterClass.Knight:
                characterClassData = classData[0].characterClassData; 
                weaponData = classData[0].weaponData;
                break;
            case CharacterClass.Archer:
                characterClassData = classData[1].characterClassData;
                weaponData = classData[1].weaponData;
                break;
            case CharacterClass.Mage:
                characterClassData = classData[2].characterClassData;
                weaponData = classData[2].weaponData;
                break;
            default: 
                break;
        }
    }

    [System.Serializable]
    public class ClassData
    {
        public CharacterClassData characterClassData;
        public WeaponData weaponData;
    }
}
