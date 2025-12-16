using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewMuaTen", menuName = "Skill/MuaTen")]
public class SkillArrowRain : SkillBase
{
    public float damageMultiplier;              
    public float areaRadius = 4f;               
    public float radius = 5f;             
    public LayerMask enemyLayer;
    public GameObject skillEffect1;
    public GameObject skillEffect2;
}