using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private AnimatorStateInfo enemyInfo;
    public GameObject player;
    private float distance;
    private bool isAttacking = false;
    public float attackRange = 2.0f;
    public float runRange = 12.0f;
    private WaitForSeconds lookTime = new WaitForSeconds(2);
    private EnemyStats enemyStats;

    // Retreat Settings
    [Header("Retreat Settings")]
    public float retreatHealthThreshold = 0.2f; // 20%
    public float retreatSpeed = 2f;
    public float retreatStopDistance = 15f;
    public float healDelay = 3f;

    private bool isRetreating = false;
    private bool isHealing = false;
    private float originalSpeed;

    // Fireball config
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform[] fireballSpawnPoints;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;


    public Collider weaponCollider;
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

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        distance = Vector3.Distance(transform.position, player.transform.position);

        // Retreat logic
        if (!isHealing && enemyStats.MaxHealth / enemyStats.currentHealth <= retreatHealthThreshold)
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
            nav.isStopped = true;

            if (distance < attackRange && enemyInfo.IsTag("nonattack"))
            {
                if (!isAttacking)
                {
                    isAttacking = true;
                    anim.SetTrigger("attack");
                    StartCoroutine(LookAtPlayer());
                }
            }

            if (distance < attackRange && enemyInfo.IsTag("attack"))
            {
                isAttacking = false;
            }
        }
        else
        {
            nav.isStopped = false;
            nav.destination = player.transform.position;
        }
    }

    void HandleRetreat()
    {

        isRetreating = true;
        nav.speed = retreatSpeed;

        Vector3 retreatDir = (transform.position - player.transform.position).normalized;
        Vector3 retreatTarget = transform.position + retreatDir * 5f;

        nav.isStopped = false;
        nav.SetDestination(retreatTarget);
        anim.SetBool("running", true);

        // 👉 Hướng ngược lại player
        if (retreatDir != Vector3.zero)
        {
            Quaternion backwardRotation = Quaternion.LookRotation(retreatDir); // hướng tránh xa player
            transform.rotation = Quaternion.Slerp(transform.rotation, backwardRotation, Time.deltaTime * 10f);
        }

        if (distance > retreatStopDistance)
        {
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

    IEnumerator LookAtPlayer()
    {
        yield return lookTime;
        transform.LookAt(player.transform);
    }

    void FireProjectile()
    {
        if (fireballPrefab == null || fireballSpawnPoints.Length == 0 || player == null) return;

        foreach (Transform spawnPoint in fireballSpawnPoints)
        {
            GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position, Quaternion.identity);
            Vector3 start = spawnPoint.position;
            Vector3 end = player.transform.position;

            float timeToTarget;
            Vector3 velocity = CalculateFireballVelocity(start, end, fireballArcHeight, out timeToTarget, fireballGravity);

            FireballMover mover = fireball.GetComponent<FireballMover>();
            if (mover != null)
            {
                // ✅ Truyền enemyStats vào
                mover.Initialize(start, velocity, fireballGravity, timeToTarget, enemyStats, player.transform);
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

}
