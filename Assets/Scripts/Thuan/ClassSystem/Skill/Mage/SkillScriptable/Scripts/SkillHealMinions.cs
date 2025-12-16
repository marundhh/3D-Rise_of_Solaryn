using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "NewHealMinions", menuName = "Skill/HealMinions")]
public class SkillHealMinions : SkillBase
{
    public float healAmount = 50f;             // Lượng máu hồi mỗi đệ
    public float healRadius = 10f;             // Phạm vi tìm đệ để hồi máu
    public LayerMask minionLayer;              // Layer để xác định đệ
}
