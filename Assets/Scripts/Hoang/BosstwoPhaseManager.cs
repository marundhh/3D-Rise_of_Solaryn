using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BosstwoPhaseManager : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private AnimatorStateInfo enemyInfo;
    private Transform currentTarget;
    private float distance;
    private bool isAttacking = false;

    public float attackRange = 2.0f;
    public float runRange = 12.0f;
    private WaitForSeconds lookTime = new WaitForSeconds(2);
    private EnemyStats enemyStats;

    // Fireball config
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform[] fireballSpawnPoints;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;

    // Phase control
    private bool hasPhaseChanged = false;
    private bool isPhaseChanging = false;

    [SerializeField] private float speedPhase1 = 3.5f;
    [SerializeField] private float speedPhase2 = 6f;

    private bool useAttack1Next = true;

    [SerializeField] private GameObject phaseChangeEffect;
    [SerializeField] private Transform effectSpawnPoint;

    // Vũ khí
    [SerializeField] private Collider weaponCollider1; // Phase 1
    [SerializeField] private Collider weaponCollider2; // Phase 2

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        nav.avoidancePriority = Random.Range(5, 75);
        nav.speed = speedPhase1;

        DisableWeapons();
    }

    void Update()
    {
        if (enemyStats == null || enemyStats.isDead || nav == null || !nav.enabled || !nav.isOnNavMesh)
            return;

        currentTarget = FindNearestTarget(new string[] { "Player", "Minion" });
        if (currentTarget == null) return;

        if (isPhaseChanging) return;

        Vector3 velocity = nav.velocity;
        bool isRunning = velocity.magnitude > 0.1f;
        anim.SetBool("running", isRunning);
        isAttacking = isRunning ? false : isAttacking;

        enemyInfo = anim.GetCurrentAnimatorStateInfo(0);
        distance = Vector3.Distance(transform.position, currentTarget.position);

        // Chuyển phase nếu máu thấp
        if (!hasPhaseChanged && enemyStats.currentHealth <= enemyStats.MaxHealth * 0.5f)
        {
            ChangePhase();
            return;
        }

        if (distance < attackRange || distance > runRange)
        {
            nav.isStopped = true;

            if (distance < attackRange && enemyInfo.IsTag("nonattack"))
            {
                if (!isAttacking)
                {
                    isAttacking = true;

                    if (hasPhaseChanged)
                    {
                        if (useAttack1Next)
                        {
                            anim.SetTrigger("attack1");
                        }
                        else
                        {
                            anim.SetTrigger("attack2");
                            // FireProjectile(); nếu muốn bắn ở attack2
                        }

                        useAttack1Next = !useAttack1Next;
                    }
                    else
                    {
                        anim.SetTrigger("attack1");
                    }

                    StartCoroutine(LookAtTarget());
                }
            }

            if (distance < attackRange && enemyInfo.IsTag("attack"))
            {
                isAttacking = false;
            }
        }
        else
        {
            if (nav.isOnNavMesh)
            {
                nav.isStopped = false;
                nav.destination = currentTarget.position;
            }
        }
    }

    Transform FindNearestTarget(string[] tags)
    {
        float shortestDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (string tag in tags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject t in targets)
            {
                float dist = Vector3.Distance(transform.position, t.transform.position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestTarget = t.transform;
                }
            }
        }

        return nearestTarget;
    }

    void ChangePhase()
    {
        hasPhaseChanged = true;
        isPhaseChanging = true;
        nav.isStopped = true;
        enemyStats.isInvincible = true;

        anim.SetTrigger("scream");
        Debug.Log("Boss đang chuyển phase...");

        StartCoroutine(PhaseTransition());
    }

    IEnumerator PhaseTransition()
    {
        yield return new WaitForSeconds(3f);

        isPhaseChanging = false;
        nav.isStopped = false;
        nav.speed = speedPhase2;
        enemyStats.isInvincible = false;

        attackRange = 10f;

        Debug.Log("Boss đã chuyển sang Phase 2.");
    }

    void FireProjectile()
    {
        if (!hasPhaseChanged || fireballPrefab == null || fireballSpawnPoints.Length == 0 || currentTarget == null) return;

        foreach (Transform spawnPoint in fireballSpawnPoints)
        {
            GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);
            Vector3 start = spawnPoint.position;
            Vector3 end = currentTarget.position;

            float timeToTarget;
            Vector3 velocity = CalculateFireballVelocity(start, end, fireballArcHeight, out timeToTarget, fireballGravity);

            FireballMover mover = fireball.GetComponent<FireballMover>();
            if (mover != null)
            {
                mover.Initialize(start, velocity, fireballGravity, timeToTarget, enemyStats, currentTarget);
            }
        }
    }

    Vector3 CalculateFireballVelocity(Vector3 start, Vector3 end, float arcHeight, out float timeToTarget, Vector3 gravity)
    {
        Vector3 displacement = end - start;
        Vector3 gravityDir = gravity.normalized;
        Vector3 up = -gravityDir;

        float g = gravity.magnitude;
        float verticalOffset = Vector3.Dot(displacement, up);
        float horizontalDistance = (displacement - verticalOffset * up).magnitude;

        float h = Mathf.Max(arcHeight, verticalOffset);
        float vy = Mathf.Sqrt(2 * g * h);
        float timeUp = vy / g;
        float timeDown = Mathf.Sqrt(2 * Mathf.Abs(h - verticalOffset) / g);
        timeToTarget = timeUp + timeDown;

        Vector3 horizontalVelocity = (displacement - verticalOffset * up) / timeToTarget;
        Vector3 verticalVelocity = vy * up;

        return horizontalVelocity + verticalVelocity;
    }

    IEnumerator LookAtTarget()
    {
        yield return lookTime;
        if (currentTarget != null)
            transform.LookAt(currentTarget.position);
    }

    public void PlayPhaseChangeEffect()
    {
        if (phaseChangeEffect != null && effectSpawnPoint != null)
        {
            Instantiate(phaseChangeEffect, effectSpawnPoint.position, Quaternion.identity);
        }
    }

    // ---------------- Animation Events ----------------

    public void EnableWeapon1()
    {
        if (weaponCollider1 != null)
            weaponCollider1.enabled = true;
    }

    public void EnableWeapon2()
    {
        if (hasPhaseChanged && weaponCollider2 != null)
            weaponCollider2.enabled = true;
    }

    public void DisableWeapons()
    {
        if (weaponCollider1 != null)
            weaponCollider1.enabled = false;

        if (weaponCollider2 != null)
            weaponCollider2.enabled = false;
    }
}
