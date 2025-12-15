using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuffTdandTb", menuName = "Skill/BuffTdandTb")]
public class SkillOverdrive : SkillBase
{
    public float attackSpeedBonus = 0.3f;    // +30% tốc độ bắn
    public float moveSpeedBonus = 0.2f;      // +20% tốc độ chạy
    public float damageReduction = 0.1f;     // -10% sát thương nhận vào

    
}
