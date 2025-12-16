using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private AnimatorStateInfo enemyInfo;
    private Transform currentTarget;
    private float distance;
    private bool isAttacking = false;

    [Header("Target Settings")]
    public float attackRange = 2.0f;
    public float runRange = 12.0f;

    private WaitForSeconds lookTime = new WaitForSeconds(2);
    private EnemyStats enemyStats;

    [Header("Retreat Settings")]
    public float retreatHealthThreshold = 0.2f;
    public float retreatSpeed = 2f;
    public float retreatStopDistance = 15f;
    public float healDelay = 3f;

    private bool isRetreating = false;
    private bool isHealing = false;
    private float originalSpeed;

    [Header("Fireball Settings")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform[] fireballSpawnPoints;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;

    public Collider weaponCollider;

    private bool battleReported = false;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        nav.avoidancePriority = Random.Range(5, 75);
        originalSpeed = nav.speed;
    }

    void Update()
    {
        if (enemyStats == null || enemyStats.isDead) return;

        // 🔄 Luôn tìm mục tiêu gần nhất mỗi frame
        currentTarget = FindNearestTarget(new string[] { "Player", "Minion" });
        if (currentTarget == null)
        {
            ReportExitCombat();
            return;
        }

        distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance <= runRange)
        {
            ReportEnterCombat();
        }
        else
        {
            ReportExitCombat();
        }

        if (!isHealing && ((float)enemyStats.currentHealth / enemyStats.MaxHealth) <= retreatHealthThreshold)
        {
            HandleRetreat();
            return;
        }

        Vector3 velocity = nav.velocity;
        bool isRunning = velocity.magnitude > 0.1f;
        anim.SetBool("running", isRunning);
        isAttacking = isRunning ? false : isAttacking;

        enemyInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (distance < attackRange || distance > runRange)
        {
            if (nav.isOnNavMesh)
                nav.isStopped = true;


            if (distance < attackRange && enemyInfo.IsTag("nonattack"))
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    anim.SetTrigger("attack");
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

    void HandleRetreat()
    {
        isRetreating = true;
        nav.speed = retreatSpeed;

        Vector3 retreatDir = (transform.position - currentTarget.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDir * 5f;

        if (nav.isOnNavMesh)
        {
            nav.isStopped = false;
            nav.SetDestination(retreatTarget);
        }

        anim.SetBool("running", true);

        if (retreatDir != Vector3.zero)
        {
            Quaternion backwardRotation = Quaternion.LookRotation(retreatDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, backwardRotation, Time.deltaTime * 10f);
        }

        if (distance > retreatStopDistance)
        {
            if (nav.isOnNavMesh)
                nav.isStopped = true;
            anim.SetBool("running", false);
            isRetreating = false;
            StartCoroutine(HealAfterDelay());
        }
    }

    IEnumerator HealAfterDelay()
    {
        isHealing = true;
        yield return new WaitForSeconds(healDelay);

        if (enemyStats != null)
        {
            enemyStats.HealToFull();
        }

        nav.speed = originalSpeed;
        isHealing = false;
    }

    public void FireProjectileFromAnimationEvent()
    {
        FireProjectile();
    }

    IEnumerator LookAtTarget()
    {
        yield return lookTime;

        if (currentTarget != null)
            transform.LookAt(currentTarget.position);
    }

    void FireProjectile()
    {
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

    #region Combat Audio (Enter/Exit battle reporting)
   

    private void OnEnable() => battleReported = false;

    private void ReportEnterCombat()
    {
        if (!battleReported)
        {
            battleReported = true;
            AudioManager.Instance?.EnterBattle();
        }
    }

    private void ReportExitCombat()
    {
        if (battleReported)
        {
            battleReported = false;
            AudioManager.Instance?.ExitBattle();
        }
    }

    private void OnDisable() => ReportExitCombat();
    #endregion
}