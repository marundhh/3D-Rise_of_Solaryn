using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewBanTen", menuName = "Skill/BanTen")]
public class SkillTripleArrow : SkillBase
{
    public float damageMultiplier;
    public float coneAngle = 45f;
    public float range = 5f;
    public LayerMask enemyLayer;
    public GameObject skillEffect1;
    public GameObject skillEffect2;
    //public GameObject effectPrefab;

}
