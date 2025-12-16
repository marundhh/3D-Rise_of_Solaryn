using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionAttack : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private AnimatorStateInfo animInfo;

    public float attackRange = 1.8f;
    public float detectRange = 12f;

    private GameObject currentTarget;
    private bool isAttacking = false;

    private MinionStats minionStats;

    public Collider weaponCollider;
    
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        minionStats = GetComponent<MinionStats>();

        nav.avoidancePriority = Random.Range(10, 80);
    }

    void Update()
    {
        if (minionStats == null || minionStats.isDead) return;

        currentTarget = FindClosestEnemy();
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        animInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (distance <= attackRange)
        {
            nav.isStopped = true;
            anim.SetBool("running", false);

            transform.LookAt(currentTarget.transform); // quay mặt

            if (!isAttacking && animInfo.IsTag("nonattack"))
            {
                anim.SetTrigger("attack");
                isAttacking = true;
            }

            if (animInfo.IsTag("attack"))
            {
                isAttacking = false;
            }
        }
        else if (distance <= detectRange)
        {
            nav.isStopped = false;
            nav.SetDestination(currentTarget.transform.position);
            anim.SetBool("running", true);
        }
        else
        {
            nav.isStopped = true;
            anim.SetBool("running", false);
        }
    }

    public void SetTarget(GameObject target)
    {
        currentTarget = target;
    }

    public void ClearTarget()
    {
        currentTarget = null;
    }

    GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDist = Mathf.Infinity;
        GameObject closest = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null && !stats.isDead)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
        }

        return closest;
    }

    // gọi từ animation event
    public void EnableWeaponHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;

    }

    public void DisableWeaponHitbox()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;

    }
}
