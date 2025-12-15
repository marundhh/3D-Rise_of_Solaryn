using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewXoayKiem", menuName = "Skill/XoayKiem")]
public class SkillSpinSword : SkillBase 
{
        public float damageMultiplier;
        public float radiusDamage = 2f;
        public LayerMask enemyLayer;
}