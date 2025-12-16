using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewSummonMinions", menuName = "Skill/SummonMinions")]
public class SkillSummonMinions : SkillBase
{
    public int summonCount = 3;                // Số đệ triệu hồi mỗi lần
    public int maxMinions = 9;                 // Tối đa số đệ có thể tồn tại
    public GameObject minionPrefab;            // Prefab của đệ
    public float summonRadius = 2f;            // Khoảng cách sinh ra xung quanh pháp sư
    
    public float minionLifetime = 15f;  // Giây. 0 = sống vô thời hạn (không auto-destroy)

    public static int MaxMinionsGlobal { get; private set; }

    private void OnEnable()
    {
        MaxMinionsGlobal = maxMinions;
    }

    private void OnValidate()
    {
        if (maxMinions < 0) maxMinions = 0;
        if (summonCount < 0) summonCount = 0;
        if (summonRadius < 0f) summonRadius = 0f;
        if (minionLifetime < 0f) minionLifetime = 0f;
    }
}
