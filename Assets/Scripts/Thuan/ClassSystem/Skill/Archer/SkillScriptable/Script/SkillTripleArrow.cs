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
    public float coneAngle = 45f;              // Góc hình nón bắn
    public float slowAmount = 0.4f;            // Làm chậm (40%)
    public float slowDuration = 2f;
    public int arrowCount = 3;                 // Số mũi tên (3 mũi)
    public LayerMask enemyLayer;

    //public GameObject effectPrefab;

}
