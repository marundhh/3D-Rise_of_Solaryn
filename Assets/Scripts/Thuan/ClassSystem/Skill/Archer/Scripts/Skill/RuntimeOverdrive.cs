using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class RuntimeOverdrive : RuntimeSkillBase
{
    private PlayerStats stats;

    private SkillOverdrive skillData;
    public GameObject skillEffect;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;

    private float baseAttackSpeed;
    private float baseMoveSpeed;
    private float baseArmor;

    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeOverdrive(SkillOverdrive data, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = data;
        user = userObj;

        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffect = data.skillEffect;
        stats = user.transform.parent.GetComponent<PlayerStats>();
        skillController.skillIcon[1].sprite = skillData.SkillIcon;
    }

    public void PlaySoundEff()
    {
        audioSource.loop = false;
        audioSource.clip = skillData?.skillSoundEff;
        audioSource?.Play();
    }

    public void StopSoundEff()
    {
        audioSource.clip = skillData?.skillSoundEff;
        audioSource?.Stop();
    }

    public void PlaySkillEff()
    {
        if (runtimeSkillEff == null && skillEffect != null)
        {
            runtimeSkillEff = GameObject.Instantiate(skillEffect, user.transform.position + new Vector3 (0, 1, 0), Quaternion.identity,
                skillController.tranformSkillEffects).GetComponent<ParticleSystem>();
        }
        runtimeSkillEff?.Play();
    }

    public void StopSkillEff()
    {
        if (runtimeSkillEff != null)
            runtimeSkillEff.Stop();
    }


    public override async UniTask Activate()
    {
        if (!ManaCostProces()) return;

        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();

        if (stats == null)
        {
            Debug.LogError("CreatureStats không được tìm thấy!");
            isSpinning = false;
            return;
        }

        animator?.SetTrigger("Overdrive");
        PlaySoundEff();
        PlaySkillEff();
        StartSkillFeature();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(skillData.Duration), cancellationToken: cancelTokenSource.Token);
        }
        catch (OperationCanceledException) { }

        StopSoundEff();
        StopSkillEff();
        EndSkillFeature();

        isSpinning = false;
        await StartCooldown();
    }

    public void StartSkillFeature()
    {
        baseAttackSpeed = stats.currentAttackSpeed;
        baseMoveSpeed = stats.currentMoveSpeed;
        baseArmor = stats.currentArmor;

        stats.currentAttackSpeed = baseAttackSpeed * (1 + skillData.attackSpeedBonus);
        stats.currentMoveSpeed = baseMoveSpeed * (1 + skillData.moveSpeedBonus);
        stats.currentArmor = baseArmor + skillData.damageReduction;
    }

    public void EndSkillFeature()
    {
        stats.currentAttackSpeed = baseAttackSpeed;
        stats.currentMoveSpeed = baseMoveSpeed;
        stats.currentArmor = baseArmor;
    }
}
