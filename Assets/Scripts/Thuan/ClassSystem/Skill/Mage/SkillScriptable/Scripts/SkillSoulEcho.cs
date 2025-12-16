using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoulEcho", menuName = "Skill/SoulEcho")]
public class SkillSoulEcho : SkillBase
{
    [Range(0f, 1f)]
    public float statTransferRatio = 0.5f;     // Tỷ lệ chỉ số quái được chuyển sang đệ
    public LayerMask minionLayer;              // Layer xác định đệ
}
