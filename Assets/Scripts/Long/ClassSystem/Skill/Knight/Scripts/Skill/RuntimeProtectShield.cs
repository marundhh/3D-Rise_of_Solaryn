using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class RuntimeProtectShield : RuntimeSkillBase
{

    private SkillArmorBuff data;
    private PlayerStats stats;
    private List<ParticleSystem> magicShieldEffects;

    public MagicShieldObject magicShieldObject;
    public GameObject skillEffect;
    public override float Cooldown => data.Cooldown;
    public override float ManaCost => data.ManaCost;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;
    private GameObject runtimeMagicShieldObject;
    public RuntimeProtectShield(SkillArmorBuff data, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.data = data;
        user = userObj;

        stats = user.transform.parent.GetComponent<PlayerStats>();
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffect = data.skillEffect;
    }

    public void PlaySoundEff()
    {
        audioSource.loop = false;
        audioSource.clip = data?.skillSoundEff;
        audioSource?.Play();
    }
    public void StopSoundEff()
    {
        audioSource.clip = data?.skillSoundEff;
        audioSource?.Stop();
    }
    public void PlaySkillEff()
    {
        if (runtimeSkillEff == null)
        {
            runtimeSkillEff = GameObject.Instantiate(skillEffect, skillController.tranformSkillEffects).GetComponent<ParticleSystem>();
        }

        runtimeSkillEff.Play();
    }
    public void PlayMagicShielObject()
    {
        if (runtimeMagicShieldObject == null)
        {
            runtimeMagicShieldObject = GameObject.Instantiate(data.magicShieldObject, skillController.tranformSkillEffects);
            runtimeMagicShieldObject.transform.position += new Vector3(0, 1, 0);
        }

        runtimeMagicShieldObject.GetComponent<MagicShieldObject>().MagicShieldCast();
    }
    public void StopMagicShielObject()
    {
        runtimeMagicShieldObject.GetComponent<MagicShieldObject>().MagicShieldEnd();
    }
    public override async UniTask Activate()
    {
        if (!ManaCostProces()) return;
        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();

        animator?.SetBool("Skill2", true);
        PlaySoundEff();
        PlayMagicShielObject();
        PlaySkillEff();
        StartSkilFeature();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: cancelTokenSource.Token);
        }
        catch (OperationCanceledException) { }

        animator?.SetBool("Skill2", false);

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(data.Duration), cancellationToken: cancelTokenSource.Token);
        }
        catch (OperationCanceledException) { }
        if (magicShieldEffects != null)
            foreach (var ps in magicShieldEffects) ps.Stop();
        isSpinning = false;
        StopMagicShielObject();
        StopSoundEff();
        EndSkillFreatur(); 
        await StartCooldown();
    }

    private float baseCurrentArmor;
    public void StartSkilFeature()
    {
        baseCurrentArmor = stats.currentArmor;
        stats.currentArmor *= data.armorMultiply;
        PlayerStats.instance = stats;
    }

    public void EndSkillFreatur()
    {
        stats.currentArmor = baseCurrentArmor;
        PlayerStats.instance = stats;
    }
}
