using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RuntimeSoulEcho : RuntimeSkillBase 
{
    private SkillSoulEcho skillData;
    public GameObject skillEffects;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;

    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeSoulEcho(SkillSoulEcho skillData, GameObject userObj, SkillController control)
    {
        this.skillData = skillData;
        skillEffects = skillData.skillEffect;

        user = userObj;
        skillController = control;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillController.skillIcon[2].sprite = skillData.SkillIcon;
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
        cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        var soulData = SoulManager.Instance?.PopLastSoul();
        if (soulData == null)
        {
            Debug.Log("Không có linh hồn nào để triệu hồi.");
            return;
        }

        if (!ManaCostProces()) return;

        animator?.SetTrigger("Soul");
        PlaySoundEff();
        PlaySkillEff();

        await UniTask.Delay(400, cancellationToken: token);

        Collider[] minions = Physics.OverlapSphere(user.transform.position, 20f, skillData.minionLayer);


        foreach (var minion in minions)
        {
            var stats = minion.GetComponent<MinionStats>();
            if (stats != null)
            {
                stats.BuffFromSoul(soulData, skillData.statTransferRatio, skillData.Duration);
            }
        }

        StopSoundEff();
        StopSkillEff();
        await StartCooldown();
    }
}
