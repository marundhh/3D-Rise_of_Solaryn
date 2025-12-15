using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossPhaseManager : MonoBehaviour
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

    // Fireball config
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPoint;
    [SerializeField] float fireballArcHeight = 5f;
    [SerializeField] Vector3 fireballGravity = Vector3.down * 10f;

    // Phase Control
    private bool hasPhaseChanged = false;
    private bool isPhaseChanging = false;

    [SerializeField] private float speedPhase1 = 3.5f;
    [SerializeField] private float speedPhase2 = 6f;

    // Attack Switching
    private bool useAttack1Next = true;

    [SerializeField] private GameObject phaseChangeEffect;
    [SerializeField] private Transform effectSpawnPoint;

    public Collider weaponCollider;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
        nav.avoidancePriority = Random.Range(5, 75);
        nav.speed = speedPhase1;
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
    void Update()
    {
        if (enemyStats == null) return;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (isPhaseChanging) return;

        Vector3 velocity = nav.velocity;
        bool isRunning = velocity.magnitude > 0.1f;
        anim.SetBool("running", isRunning);
        isAttacking = isRunning ? false : isAttacking;

        enemyInfo = anim.GetCurrentAnimatorStateInfo(0);
        distance = Vector3.Distance(transform.position, player.transform.position);

        // Kiểm tra chuyển phase
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
                        // Phase 2: Luân phiên attack 1 và attack 2
                        if (useAttack1Next)
                        {
                            anim.SetTrigger("attack1");
                        }
                        else
                        {
                            anim.SetTrigger("attack2");
                        }
                        useAttack1Next = !useAttack1Next; // Đổi lượt attack
                    }
                    else
                    {
                        // Phase 1: Chỉ attack 1
                        anim.SetTrigger("attack1");
                    }

                    StartCoroutine(LookAtPlayer());
                    FireProjectile();
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
        yield return new WaitForSeconds(3f); // Thời gian animation scream

        isPhaseChanging = false;
        nav.isStopped = false;
        nav.speed = speedPhase2;

        enemyStats.isInvincible = false;
        Debug.Log("Boss đã chuyển phase xong.");
    }

    IEnumerator LookAtPlayer()
    {
        yield return lookTime;
        transform.LookAt(player.transform);
    }

    void FireProjectile()
    {
        if (fireballPrefab == null || fireballSpawnPoint == null || player == null) return;

        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
        Vector3 start = fireballSpawnPoint.position;
        Vector3 end = player.transform.position;

        float timeToTarget;
        Vector3 velocity = CalculateFireballVelocity(start, end, fireballArcHeight, out timeToTarget, fireballGravity);

        FireballMover mover = fireball.GetComponent<FireballMover>();
        if (mover != null)
        {
            // ✅ Truyền thêm `enemyStats` để lấy damage
            mover.Initialize(start, velocity, fireballGravity, timeToTarget, enemyStats, player.transform);
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
    // Gọi từ Animation Event
    public void PlayPhaseChangeEffect()
    {
        if (phaseChangeEffect != null && effectSpawnPoint != null)
        {
            Instantiate(phaseChangeEffect, effectSpawnPoint.position, Quaternion.identity);
        }
    }

}
