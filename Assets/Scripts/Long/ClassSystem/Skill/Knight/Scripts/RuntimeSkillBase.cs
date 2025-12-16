using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public abstract class RuntimeSkillBase : IRuntimeSkill
{
    protected GameObject user;
    protected SkillController skillController;
    protected Animator animator;
    protected AudioSource audioSource;
    protected CancellationTokenSource cancelTokenSource;
    protected bool isOnCooldown = false;
    protected bool isSpinning = false;

    public abstract float Cooldown { get; }
    public abstract float ManaCost { get; }
    public abstract UniTask Activate();

    public void UseSkill()
    {
        if (!isOnCooldown)
            if(!isSpinning)
                Activate().Forget();
            else
                Cancel();
    }
    public void Cancel()
    {
        cancelTokenSource?.Cancel();
    }

    public bool ManaCostProces()
    {
        if(PlayerStats.instance.currentMana >= ManaCost)
        {
            PlayerStats.instance.currentMana -= ManaCost;
            return true;
        }
        return false;
    }

    public virtual async UniTask StartCooldown()
    {
        isOnCooldown = true;

        float reduction = Mathf.Clamp01(PlayerStats.instance?.currentCooldownReduction ?? 0f);
        float cd = Mathf.Max(0.01f, Cooldown * (1f - reduction));

        // Map UI theo index 
        int index = skillController.runtimeSkills.IndexOf(this);

        if (index >= 0 && index < skillController.skillIconCooldown.Count)
        {
            var img = skillController.skillIconCooldown[index];
            var txt = skillController.skillTextCooldown[index];

            float elapsed = 0f;
            //float cd = Cooldown;

            while (elapsed < cd)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / cd);
                if (img) img.fillAmount = 1f - t;
                if (txt) txt.text = (cd - elapsed).ToString("0.0");

                await UniTask.Yield();
            }

            if (img) img.fillAmount = 0f;
            if (txt) txt.text = "";
        }

        isOnCooldown = false;
    }
}
