using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossthreePhaseManager : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private AnimatorStateInfo enemyInfo;
    private Transform currentTarget;
    private float distance;
    private bool isAttacking = false;
    public float attackRange = 4.0f;
    public float runRange = 12.0f;
    private WaitForSeconds lookTime = new WaitForSeconds(2);
    private EnemyStats enemyStats;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform[] fireballSpawnPoints;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;

    private bool hasPhaseChanged = false;
    private bool hasPhase3Changed = false;
    private bool isPhaseChanging = false;

    [SerializeField] private float speedPhase1 = 3.5f;
    [SerializeField] private float speedPhase2 = 6f;

    private bool useAttack1Next = true;

    [SerializeField] private GameObject phaseChangeEffect;
    [SerializeField] private Transform effectSpawnPoint;

    public Collider weaponCollider;

    [SerializeField] private GameObject phase3SpawnerPrefab;
    [SerializeField] private Transform phase3SpawnPoint;
    private GameObject phase3SpawnedObject;
    [SerializeField] private GameObject modelToHide;

    
    [Header("Teleport Behind Target Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float teleportChance = 0.3f; 
    [SerializeField] private float behindDistance = 2f;   
    [SerializeField] private float yOffset = 0.5f;       

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        nav.avoidancePriority = Random.Range(5, 75);
        nav.speed = speedPhase1;

        if (phase3SpawnPoint == null)
        {
            GameObject foundByTag = GameObject.FindWithTag("Phase3SpawnPoint");
            if (foundByTag != null)
            {
                phase3SpawnPoint = foundByTag.transform;
                Debug.Log("Phase3 spawn point được tìm bằng tag.");
            }
            else
            {
                GameObject foundByName = GameObject.Find("Phase3SpawnPoint");
                if (foundByName != null)
                {
                    phase3SpawnPoint = foundByName.transform;
                    Debug.Log("Phase3 spawn point được tìm bằng tên.");
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy Phase3SpawnPoint bằng tag hoặc tên.");
                }
            }
        }
    }

    void Update()
    {
        if (enemyStats == null || enemyStats.isDead || nav == null || !nav.enabled || !nav.isOnNavMesh)
            return;

        // Luôn tìm mục tiêu gần nhất
        currentTarget = FindNearestTarget(new string[] { "Player", "Minion" });
        if (currentTarget == null) return;

        Vector3 velocity = nav.velocity;
        bool isRunning = velocity.magnitude > 0.1f;
        anim.SetBool("running", isRunning);
        isAttacking = isRunning ? false : isAttacking;

        enemyInfo = anim.GetCurrentAnimatorStateInfo(0);
        distance = Vector3.Distance(transform.position, currentTarget.position);

        float healthPercent = enemyStats.currentHealth / enemyStats.MaxHealth;

        if (!hasPhase3Changed && healthPercent <= 0.5f)
        {
            StartCoroutine(EnterPhase3());
            return;
        }

        if (!hasPhaseChanged && healthPercent <= 0.8f)
        {
            ChangePhase();
            return;
        }

        if (isPhaseChanging) return;

        if (distance < attackRange || distance > runRange)
        {
            nav.isStopped = true;

            if (distance < attackRange && enemyInfo.IsTag("nonattack"))
            {
                if (!isAttacking)
                {
                    isAttacking = true;

                    if (Random.value < teleportChance)
                    {
                        TeleportBehindTarget(currentTarget);
                    }

                    if (hasPhase3Changed)
                    {
                        anim.SetTrigger("attack2");
                    }
                    else if (hasPhaseChanged)
                    {
                        if (useAttack1Next)
                            anim.SetTrigger("attack1");
                        else
                            anim.SetTrigger("attack2");

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
        else if (nav.enabled && nav.isOnNavMesh)
        {
            nav.isStopped = false;
            nav.SetDestination(currentTarget.position);
        }
    }

    void ChangePhase()
    {
        hasPhaseChanged = true;
        isPhaseChanging = true;
        nav.isStopped = true;
        enemyStats.isInvincible = true;

        anim.SetTrigger("scream");
        Debug.Log("Boss đang chuyển sang Phase 2...");

        StartCoroutine(PhaseTransition());
    }

    IEnumerator PhaseTransition()
    {
        yield return new WaitForSeconds(3f);
        isPhaseChanging = false;
        nav.isStopped = false;
        nav.speed = speedPhase2;
        enemyStats.isInvincible = false;
        attackRange = 4f;
        Debug.Log("Boss đã chuyển sang Phase 2.");
    }

    IEnumerator EnterPhase3()
    {
        hasPhase3Changed = true;
        isPhaseChanging = true;

        nav.isStopped = true;
        enemyStats.isInvincible = true;

        anim.SetTrigger("scream");
        Debug.Log("Boss đang chuyển sang Phase 3...");

        yield return new WaitForSeconds(3f);

        if (phase3SpawnerPrefab != null && phase3SpawnPoint != null)
        {
            phase3SpawnedObject = Instantiate(phase3SpawnerPrefab, phase3SpawnPoint.position, Quaternion.identity);
        }

        if (modelToHide != null)
            modelToHide.SetActive(false);

        yield return new WaitForSeconds(35f);

        if (phase3SpawnedObject != null)
        {
            Destroy(phase3SpawnedObject);
        }

        if (modelToHide != null)
            modelToHide.SetActive(true);

        isPhaseChanging = false;
        enemyStats.isInvincible = false;

        Debug.Log("Boss đã trở lại sau Phase 3.");
        // Nếu máu boss vẫn <= 50% thì sau 30 giây nữa lại vào Phase 3 tiếp
        if (enemyStats.currentHealth / enemyStats.MaxHealth <= 0.5f && !enemyStats.isDead)
        {
            Debug.Log("Boss chuẩn bị dùng lại Phase 3 sau 30 giây...");
            yield return new WaitForSeconds(20f);
            if (enemyStats.currentHealth / enemyStats.MaxHealth <= 0.5f && !enemyStats.isDead)
            {
                StartCoroutine(EnterPhase3());
            }
        }
    }

    void FireProjectile()
    {
        if (!hasPhaseChanged) return;
        if (fireballPrefab == null || fireballSpawnPoints.Length == 0 || currentTarget == null) return;

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

    public void PlayPhaseChangeEffect()
    {
        if (phaseChangeEffect != null && effectSpawnPoint != null)
        {
            Instantiate(phaseChangeEffect, effectSpawnPoint.position, Quaternion.identity);
        }
    }

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

 
    private void TeleportBehindTarget(Transform target)
    {
        if (target == null) return;

        Vector3 behindDirection = -target.forward;
        Vector3 teleportPosition = target.position + behindDirection * behindDistance;
        teleportPosition.y += yOffset;

        transform.position = teleportPosition;
        transform.LookAt(target.position);
    }
}
