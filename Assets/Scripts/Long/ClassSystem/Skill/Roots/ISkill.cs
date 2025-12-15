using UnityEngine;

public interface ISkill
{
    string SkillName { get; }
    string SkillDescription { get; }
    float Cooldown { get; }


    /*    float DamageMultiplier { get; }*/
}