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
    public GameObject skillEffect1;
    public GameObject skillEffect2;

    private GameObject runtimeEffect1;
    private Quaternion directionToMouse;

    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;

    public RuntimeTripleArrow(SkillTripleArrow skillData, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = skillData;
        user = userObj;
        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffect1 = skillData.skillEffect1;
        skillEffect2 = skillData.skillEffect2;
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

    private void PlayEffect1_FollowPlayer()
    {
        if (runtimeEffect1 == null)
        {
            runtimeEffect1 = GameObject.Instantiate(skillEffect1, skillController.tranformSkillEffects);
        }

        runtimeEffect1.transform.SetLocalPositionAndRotation(Vector3.up, directionToMouse * Quaternion.Euler(0, -90f, 0));

        var ps = runtimeEffect1.GetComponent<ParticleSystem>();
        ps?.Play();
    }

    private void PlayEffect2_AtDirection()
    {
        var effect2 = GameObject.Instantiate(skillEffect2);
        effect2.transform.SetPositionAndRotation(
            user.transform.position + new Vector3(0, 1, 0),
            directionToMouse * Quaternion.Euler(0, -90f, 0)
        );

        var ps = effect2.GetComponent<ParticleSystem>();
        ps?.Play();
    }

    public override async UniTask Activate()
    {
        if (!ManaCostProces()) return;

        RotateToMouse();


        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();

        var token = cancelTokenSource.Token;

        animator?.SetTrigger("TripleArrow");
        PlaySoundEff();
        PlayEffect1_FollowPlayer();

        var waitTask = UniTask.Delay(TimeSpan.FromSeconds(2.55f));
        var cancelTask = UniTask.WaitUntilCanceled(token);         

        int winner = await UniTask.WhenAny(waitTask, cancelTask);
        if (winner == 1)
        {
            isSpinning = false;
            StopSoundEff();
            var ps1 = runtimeEffect1?.GetComponent<ParticleSystem>();
            ps1?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            cancelTokenSource?.Dispose();
            cancelTokenSource = null;

            await StartCooldown();
            return;
        }

        PlayEffect2_AtDirection();



        var dmgTask = DoTripleDamage(skillData.Duration, token);
        var delayTask = UniTask.Delay(TimeSpan.FromSeconds(skillData.Duration), cancellationToken: token);

        try
        {
            await UniTask.WhenAll(dmgTask, delayTask);
        }
        catch (OperationCanceledException) { }

        StopSoundEff();
        isSpinning = false;        

        await StartCooldown();
    }

    public Color gizmoColor = new Color(1f, 0f, 0f, 0.4f);
    private async UniTask DoTripleDamage(float duration, CancellationToken token)
    {
        Vector3 originOffset = Vector3.up;
        float angleHalf = skillData.coneAngle / 2f;
        float range = skillData.range;

        Vector3 origin = user.transform.position + originOffset;
        Vector3 forward = user.transform.forward;

        foreach (var hit in Physics.OverlapSphere(origin, range, skillData.enemyLayer))
        {
            if (hit.gameObject == user) continue;

            Vector3 dir = (hit.transform.position - origin).normalized;
            if (Vector3.Angle(forward, dir) <= angleHalf)
            {
                var enemy = hit.GetComponentInParent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.TakeDamage(PlayerStats.instance.currentPhysicalDamage);
                }
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
    }


    #region Rotate To Mouse
    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, user.transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPos = ray.GetPoint(distance);
            Vector3 direction = (mouseWorldPos - user.transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                directionToMouse = Quaternion.LookRotation(direction);
                user.transform.rotation = directionToMouse;
            }

            Debug.DrawLine(user.transform.position, mouseWorldPos, Color.red, 1f);
            Debug.DrawRay(mouseWorldPos, Vector3.up * 2f, Color.green, 1f);
        }
    }
    #endregion
}
