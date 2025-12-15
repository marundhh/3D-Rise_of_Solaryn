using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public interface IRuntimeSkill 
{
    void UseSkill();
    void Cancel();
    float Cooldown { get; }
    UniTask StartCooldown();
}
