using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossonePhaseManager : MonoBehaviour
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

    // Fireball
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPoint;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;

    // Phase control
    private bool hasPhaseChanged = false;
    private bool hasPhase3Changed = false; // Thêm cờ kiểm tra phase 3
    private bool isPhaseChanging = false;

    [SerializeField] private float speedPhase1 = 3.5f;
    [SerializeField] private float speedPhase2 = 6f;

    private bool useAttack1Next = true;

    [SerializeField] private GameObject phaseChangeEffect;
    [SerializeField] private Transform effectSpawnPoint;

    // Vũ khí
    [SerializeField] private Collider weaponCollider1;
    [SerializeField] private Collider weaponCollider2;

    // Phase 3
    [SerializeField] private GameObject phase3SpawnerPrefab;
    [SerializeField] private Transform phase3SpawnPoint;
    private GameObject phase3SpawnedObject;
    [SerializeField] private GameObject modelToHide;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        nav.avoidancePriority = Random.Range(5, 75);
        nav.speed = speedPhase1;

        DisableWeapons();

        // Nếu chưa gán Phase3SpawnPoint thì tìm
        if (phase3SpawnPoint == null)
        {
            GameObject foundByTag = GameObject.FindWithTag("BossOnePhase3SpawnPoint");
            if (foundByTag != null)
            {
                phase3SpawnPoint = foundByTag.transform;
            }
            else
            {
                GameObject foundByName = GameObject.Find("BossOnePhase3SpawnPoint");
                if (foundByName != null)
                    phase3SpawnPoint = foundByName.transform;
            }
        }
    }

    void Update()
    {
        if (enemyStats == null || enemyStats.isDead || nav == null || !nav.enabled || !nav.isOnNavMesh)
            return;

        currentTarget = FindNearestTarget(new string[] { "Player", "Minion" });
        if (currentTarget == null) return;

        Vector3 velocity = nav.velocity;
        bool isRunning = velocity.magnitude > 0.1f;
        anim.SetBool("running", isRunning);
        isAttacking = isRunning ? false : isAttacking;

        enemyInfo = anim.GetCurrentAnimatorStateInfo(0);
        distance = Vector3.Distance(transform.position, currentTarget.position);

        float healthPercent = enemyStats.currentHealth / enemyStats.MaxHealth;

        // Kiểm tra Phase 3 trước
        if (!hasPhase3Changed && healthPercent <= 0.5f)
        {
            StartCoroutine(EnterPhase3());
            return;
        }

        // Kiểm tra Phase 2
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

                    if (hasPhase3Changed)  // Phase 3
                    {
                        if (useAttack1Next)
                            anim.SetTrigger("attack1");
                        else
                            anim.SetTrigger("attack2");

                        useAttack1Next = !useAttack1Next;
                    }
                    else if (hasPhaseChanged) // Phase 2
                    {
                        if (useAttack1Next)
                            anim.SetTrigger("attack1");
                        else
                            anim.SetTrigger("attack2");

                        useAttack1Next = !useAttack1Next;
                    }
                    else // Phase 1
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
            if (nav.enabled && nav.isOnNavMesh)
            {
                nav.isStopped = false;
                nav.SetDestination(currentTarget.position);
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

        attackRange = 3f;   // ví dụ tăng từ 2.0 → 3.5
        Debug.Log("Boss đã chuyển sang Phase 2.");
    }

    IEnumerator EnterPhase3()
    {
        // Bật trạng thái Phase 3
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

        // Boss biến mất 45 giây
        yield return new WaitForSeconds(30f);

        if (phase3SpawnedObject != null)
            Destroy(phase3SpawnedObject);

        if (modelToHide != null)
            modelToHide.SetActive(true);

        attackRange = 3f;   // ví dụ tăng mạnh tầm đánh
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
