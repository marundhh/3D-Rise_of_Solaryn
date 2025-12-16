using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
public class RuntimeSummonMinions : RuntimeSkillBase
{
    private SkillSummonMinions skillData;
    public GameObject skillEffects;

    [Header("Runtime Item")]
    private ParticleSystem runtimeSkillEff;

    private static List<GameObject> currentMinions = new List<GameObject>();


    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeSummonMinions(SkillSummonMinions skillData, GameObject userObj, SkillController control)
    {
        this.skillData = skillData;
        skillEffects = skillData.skillEffect;

        user = userObj;
        skillController = control;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillController.skillIcon[0].sprite = skillData.SkillIcon;
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

        if (!CanUseSkill())
        {
            Debug.Log("Đã đủ minion, không thể triệu hồi thêm.");
            return;
        }

        animator?.SetTrigger("Summon");
        PlaySoundEff();
        PlaySkillEff();

        await UniTask.Delay(300, cancellationToken: token);

        await DoSummon(token);

        StopSoundEff();
        StopSkillEff();
        await StartCooldown();
    }

    private async UniTask DoSummon(CancellationToken token)
    {
        int availableSlots = skillData.maxMinions - currentMinions.Count;
        int toSummon = Mathf.Min(skillData.summonCount, availableSlots);

        for (int i = 0; i < toSummon; i++)
        {
            Vector3 spawnPos = user.transform.position + UnityEngine.Random.insideUnitSphere * 1.5f;
            spawnPos.y = user.transform.position.y;

            GameObject minion = GameObject.Instantiate(skillData.minionPrefab, spawnPos, Quaternion.identity);
            currentMinions.Add(minion);

            var autoUnregister = minion.AddComponent<AutoRemoveFromTracker>();
            autoUnregister.OnDestroyed += () =>
            {
                currentMinions.Remove(minion);
            };

            if (skillData.minionLifetime > 0f)
                _ = DestroyAfterDelay(minion, skillData.minionLifetime);

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: token);
        }
    }

    private async UniTaskVoid DestroyAfterDelay(GameObject minion, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        if (minion != null) GameObject.Destroy(minion);
    }

    private class AutoRemoveFromTracker : MonoBehaviour
    {
        public Action OnDestroyed;

        private void OnDestroy()
        {
            OnDestroyed?.Invoke();
        }
    }

    public static void ClearAllMinions()
    {
        foreach (var minion in currentMinions)
        {
            if (minion != null) GameObject.Destroy(minion);
        }
        currentMinions.Clear();
    }

    public static bool CanUseSkill()
    {
        return currentMinions.Count < SkillSummonMinions.MaxMinionsGlobal;
    }
}
