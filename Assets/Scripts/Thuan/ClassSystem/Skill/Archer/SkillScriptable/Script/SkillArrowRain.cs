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
    public float damageMultiplier;               // Vùng tồn tại bao lâu
    public float spawnRate = 0.2f;              // Mỗi 0.2s bắn 1 mũi tên
    public int arrowsPerSpawn = 2;              // Số mũi tên rơi mỗi lần
    public float areaRadius = 4f;               // Bán kính vùng mưa tên
    public float radius = 5f;
    public GameObject arrowPrefab;              // Prefab mũi tên rơi (hiệu ứng)
    public LayerMask enemyLayer;
    public GameObject skillEffect1;
    public GameObject skillEffect2;
}