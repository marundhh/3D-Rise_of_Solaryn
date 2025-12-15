using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class SkillController : MonoBehaviour
{
    public GameObject playerRoot;
    public Transform tranformSkillEffects;

    [Header("Skill Data")]
    public List<SkillBase> skillDataList; // list ScriptableObject

    [Header("UI for Skill")]
    public List<Image> skillIconCooldown = new List<Image>();
    public List<TextMeshProUGUI> skillTextCooldown = new List<TextMeshProUGUI>();

    public List<IRuntimeSkill> runtimeSkills = new List<IRuntimeSkill>();

    void Start()
    {

        foreach (var skillData in skillDataList)
        {
            switch (GetComponent<ICharacterClass>().CharType)
            {
                case ClassType.Archer:
                    InitArcherSkill(skillData);
                    break;
                case ClassType.Knight:
                    InitKnightSkill(skillData);
                    break;
            }
         

        }
        foreach(var skillData in runtimeSkills)
        {
        //    Debug.Log(skillData);   
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) runtimeSkills[0]?.UseSkill();
        if (Input.GetKeyDown(KeyCode.E)) runtimeSkills[1]?.UseSkill();
        if (Input.GetKeyDown(KeyCode.R)) runtimeSkills[2]?.UseSkill();
 
        transform.position = playerRoot.transform.position;
    }
    public void InitKnightSkill(SkillBase skillData)
    {
        if (skillData is SkillSpinSword spinSword)
        {
            var runtime = new RuntimeSpinSword(spinSword, playerRoot.gameObject, this);
            runtimeSkills.Add(runtime);
        }
        else if (skillData is SkillArmorBuff magicShield)
        {
            runtimeSkills.Add(new RuntimeProtectShield(magicShield, playerRoot.gameObject, this));
        }
        else if (skillData is SkillIncreaDamage increaDamage)
        {
            runtimeSkills.Add(new RuntimeIncreaseDamage(increaDamage, playerRoot.gameObject, this));
        }
    }

    public void InitArcherSkill(SkillBase skillData)
    {
        if (skillData is SkillTripleArrow trippleArrow)
        {
            var runtime = new RuntimeTripleArrow(trippleArrow, playerRoot.gameObject, this);
            runtimeSkills.Add(runtime);
        }
        else if (skillData is SkillOverdrive overDrive)
        {
            runtimeSkills.Add(new RuntimeOverdrive(overDrive, playerRoot.gameObject, this));
        }
        else if (skillData is SkillArrowRain arrowRain)
        {
            runtimeSkills.Add(new RuntimeArrowRain(arrowRain, playerRoot.gameObject, this));
        }
    }
}

