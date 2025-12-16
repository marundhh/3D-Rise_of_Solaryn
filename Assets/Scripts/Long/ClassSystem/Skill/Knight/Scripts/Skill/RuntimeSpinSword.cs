using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RuntimeSpinSword : RuntimeSkillBase
{
    private SkillSpinSword skillData;
    private GameObject skillEffects;


    [Header("Runtime Item")]
    ParticleSystem runtimeSkillEff;
    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeSpinSword(SkillSpinSword skillData, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = skillData;
        user = userObj;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffects = skillData.skillEffect;
        skillController.skillIcon[0].sprite = skillData.SkillIcon;
    }

    public void PlaySoundEff()
    {
        audioSource.loop = true;
        audioSource.clip = skillData.skillSoundEff;
        audioSource.Play();
    }
    public void StopSoundEff()
    {
        audioSource.clip = skillData.skillSoundEff;
        audioSource.Stop();
    }

    public void PlaySkillEff()
    {
        if (runtimeSkillEff == null)
        {
            runtimeSkillEff = GameObject.Instantiate(skillEffects, skillController.tranformSkillEffects).GetComponent<ParticleSystem>();
            runtimeSkillEff.transform.position += new Vector3(0, 1, 0);
        }

        else
        {
            runtimeSkillEff.Play();
        }
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

        animator?.SetBool("Skill1", true);
        PlaySoundEff();
        PlaySkillEff();

        var token = cancelTokenSource.Token;

        var dmgTask = DoSpinDamage(skillData.Duration, token);
        var delayTask = UniTask.Delay(TimeSpan.FromSeconds(skillData.Duration), cancellationToken: token);
        try
        {
            await UniTask.WhenAll(dmgTask, delayTask);
        }
        catch (OperationCanceledException) { }

        animator?.SetBool("Skill1", false);
        isSpinning = false;
        StopSoundEff();
        StopSkillEff();
        await StartCooldown();
    }

    public Color gizmoColor = new Color(1f, 0f, 0f, 0.4f);
    private async UniTask DoSpinDamage(float duration, CancellationToken token)
    {
        float elapsed = 0f;
        float tickRate = 0.5f; // mỗi 0.5s gây damage

        while (elapsed < duration)
        {
            token.ThrowIfCancellationRequested();
            elapsed += tickRate;

            // Quét enemy trong bán kính
            Collider[] hitColliders = Physics.OverlapSphere(user.transform.position, skillData.radiusDamage);


            foreach (var hit in hitColliders)
            {
                if (hit.gameObject == user) continue; 

                var enemy = hit.GetComponent<EnemyStats>(); 
                if (enemy != null)
                {
                    enemy.TakeDamage(PlayerStats.instance.currentPhysicalDamage);
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(tickRate), cancellationToken: token);
        }
    }

}
