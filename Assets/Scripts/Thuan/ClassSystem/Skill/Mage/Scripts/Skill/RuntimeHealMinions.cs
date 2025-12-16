using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RuntimeHealMinions : RuntimeSkillBase
{
    private SkillHealMinions skillData;
    public GameObject skillEffects;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;

    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeHealMinions(SkillHealMinions skillData, GameObject userObj, SkillController control)
    {
        this.skillData = skillData;
        user = userObj;
        skillController = control;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffects = skillData.skillEffect;
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
        if (!ManaCostProces()) return;

        cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        animator?.SetTrigger("Heal");
        PlaySoundEff();
        //PlaySkillEff();

        await UniTask.Delay(300, cancellationToken: token);

        Collider[] allies = Physics.OverlapSphere(user.transform.position, skillData.healRadius, skillData.minionLayer);

        foreach (var ally in allies)
        {
            var minion = ally.GetComponent<MinionStats>();
            if (minion != null)
            {
                minion.Heal(skillData.healAmount);

                //Hiển thị hiệu ứng hồi máu tại vị trí của minion
                if (skillEffects != null)
                {
                    GameObject eff = GameObject.Instantiate(skillEffects, minion.transform.position + Vector3.up * 1.2f, Quaternion.identity);
                    ParticleSystem ps = eff.GetComponent<ParticleSystem>();
                    if (ps != null) ps.Play();
                    GameObject.Destroy(eff, 2f); //Hủy sau 2 giây để tránh rác
                }
            }
        }

        StopSoundEff();
        //StopSkillEff();
        await StartCooldown();
    }
}
