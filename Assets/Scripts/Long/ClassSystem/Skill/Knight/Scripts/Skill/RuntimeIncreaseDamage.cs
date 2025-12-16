using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class RuntimeIncreaseDamage : RuntimeSkillBase
{
    private SkillIncreaDamage skillData;
    private PlayerStats stats;
    private ParticleSystem skillEffect;
    public Transform tranform;


    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeIncreaseDamage(SkillIncreaDamage data, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = data;
        user = userObj;
        //============================================//
        GetSwordAura().Forget();
        stats = user.transform.parent.GetComponent<PlayerStats>();
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillController.skillIcon[2].sprite = skillData.SkillIcon;
    }

    public async UniTask GetSwordAura()
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        catch (OperationCanceledException) { }

      
        skillEffect = user.transform.Find("Model").GetComponentInChildren<ModelRoot>().HandPosition.transform.GetComponentInChildren<ParticleSystem>();
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

    public override async UniTask Activate()
    {
        if (!ManaCostProces()) return;
        Debug.Log("Skill 3 has been used");
        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();
       
        animator?.SetBool("Skill3", true);
        PlaySoundEff();
        if (skillEffect)
        {
            skillEffect?.Play();
        }


        StartSkilFeature();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancelTokenSource.Token);
        }
        catch (OperationCanceledException) { }

        animator?.SetBool("Skill3", false);

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(skillData.Duration), cancellationToken: cancelTokenSource.Token);
        }
        catch (OperationCanceledException) { }
           
        EndSkillFreatur();

        isSpinning = false;
        StopSoundEff();
        if (skillEffect)
        {
           skillEffect?.Stop();
        }
        await StartCooldown();
    }

    private float baseCurrentDamage = 0;
    public void StartSkilFeature()
    {
        baseCurrentDamage = stats.currentPhysicalDamage;
        stats.currentPhysicalDamage *= skillData.increaDamageMultiply;
        PlayerStats.instance =  stats;
    }

    public void EndSkillFreatur()
    {
        stats.currentPhysicalDamage = baseCurrentDamage;
        PlayerStats.instance = stats;
    }
}
