using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class RuntimeArrowRain : RuntimeSkillBase
{
    private SkillArrowRain skillData;
    public GameObject skillEffect1;
    public GameObject skillEffect2;
    public override float Cooldown => skillData.Cooldown;
    public override float ManaCost => skillData.ManaCost;
    public RuntimeArrowRain(SkillArrowRain skillData, GameObject userObj, SkillController control)
    {
        skillController = control;
        this.skillData = skillData;
        user = userObj;

        animator = user.GetComponent<Animator>();
        audioSource = user.GetComponentInChildren<AudioSource>();
        skillEffect1 = skillData.skillEffect1;
        skillEffect2 = skillData.skillEffect2;
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

        Vector3? target = RotateToMouse(skillData.radius);
        if (!target.HasValue) return;

        Vector3 skillPosition = target.Value;
        Vector3 direction = (skillPosition - user.transform.position).normalized;

        isSpinning = true;
        cancelTokenSource = new CancellationTokenSource();

        animator?.SetTrigger("ArrowRain");
        PlaySoundEff();
        if (skillEffect1 != null)
          GameObject.Instantiate(skillEffect1, skillPosition, user.transform.rotation);
        if (skillEffect2 != null)
            GameObject.Instantiate(skillEffect2, user.transform.position + new Vector3(0, 2, 0), user.transform.rotation);


        var token = cancelTokenSource.Token;

        var dmgTask = DoArrowRain(skillData.Duration, token);
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


    private async UniTask DoArrowRain(float duration, CancellationToken token)
    {
        float elapsed = 0f;
        float tickRate = skillData.spawnRate;

        while (elapsed < skillData.Duration)
        {
            token.ThrowIfCancellationRequested();
            elapsed += skillData.spawnRate;
            await UniTask.Delay(TimeSpan.FromSeconds(tickRate), cancellationToken: token);
        }
    }

    #region Rotate To Mouse
    private Vector3? RotateToMouse(float radius)
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
                user.transform.rotation = Quaternion.LookRotation(direction);
            }

            // Vẽ debug phạm vi skill
            Debug.DrawLine(user.transform.position, mouseWorldPos, Color.red, 1f);
            Debug.DrawRay(mouseWorldPos, Vector3.up * 2f, Color.green, 1f);
            DebugDrawCircle(mouseWorldPos, radius, Color.cyan, 1f);

            return mouseWorldPos;
        }

        return null;
    }

    private void DebugDrawCircle(Vector3 center, float radius, Color color, float duration)
    {
        int segments = 30;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prevPoint, nextPoint, color, duration);
            prevPoint = nextPoint;
        }
    }
    #endregion
}
