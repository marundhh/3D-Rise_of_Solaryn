using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    public Animator animator;

    private bool rollingComplete = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAnimation(int anim)
    {
        switch (anim)
        {
            case 0:
                animator.SetFloat("State", 0);
                break;

            case 1:
                animator.SetFloat("State", 1);
                break;

            case 2:
                animator.SetFloat("State", 2);
                break;

            case 3:
                rollingComplete = false;
                animator.SetTrigger("Rolling");
                break;
        }
    }

    public void RollingComplete()
    {
        Debug.Log("Rolling complete");
        rollingComplete = true;
    }

    public bool CheckRollingComplete()
    {
        return rollingComplete;
    }

    public void PlayAttackCombo(int index)
    {
        if (index == 1)
            animator.SetTrigger("Attack1");
        else if (index == 2)
            animator.SetTrigger("Attack2");
    }

}
