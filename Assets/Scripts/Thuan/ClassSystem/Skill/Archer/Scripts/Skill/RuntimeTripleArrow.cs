using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public class RuntimeTripleArrow : RuntimeSkillBase
{
    private SkillTripleArrow skillData;
    private GameObject skillEffects;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;

    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeTripleArrow(SkillTripleArrow skillData, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = skillData;
        user = userObj;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffects = skillData.skillEffect;
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
        if (runtimeSkillEff == null)
        {
            runtimeSkillEff = GameObject.Instantiate(skillEffects, skillController.tranformSkillEffects).GetComponent<ParticleSystem>();
        }

        // Cập nhật vị trí và hướng theo nhân vật
        runtimeSkillEff.transform.SetPositionAndRotation(
            user.transform.position + new Vector3(0, 1, 0),
            Quaternion.LookRotation(user.transform.forward) * Quaternion.Euler(0, -90f, 0)
        );

        runtimeSkillEff.Play();
    }

    public void StopSkillEff()
    {
        if (runtimeSkillEff != null)
            runtimeSkillEff.Stop();
    }


    public override async UniTask Activate()
    {

        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;
        

        animator?.SetTrigger("TripleArrow");
        PlaySoundEff();
        PlaySkillEff();

        var delayTask = UniTask.Delay(TimeSpan.FromSeconds(skillData.Duration), cancellationToken: token);

        try
        {
            await delayTask;
        }
        catch (OperationCanceledException) { }

        isSpinning = false;
        StopSoundEff();
        StopSkillEff();

        await StartCooldown();
    }
}
