using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionAI : MonoBehaviour
{
    [Header("Settings")]
    public float detectRange = 15f;
    public float stopDistanceToEnemy = 1.8f;
    public float followPlayerDistance = 5f;

    [Header("Teleport (Catch-up) - no cooldown")]
    public bool enableTeleportOnFar = true;
    [Tooltip("0 = dùng detectRange làm ngưỡng dịch chuyển; >0 = ngưỡng riêng")]
    public float teleportDistance = 0f;
    [Tooltip("0 = dịch chuyển đúng vị trí Player; >0 = random offset quanh Player để tránh chồng hình")]
    public float teleportOffset = 0f;
    public GameObject teleportVfx;

    private GameObject targetEnemy;
    private GameObject player;

    private NavMeshAgent nav;
    private Animator anim;
    private MinionAttack attackScript;

    private float checkTimer = 0f;
    private float checkInterval = 1f;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        attackScript = GetComponent<MinionAttack>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (enableTeleportOnFar)
        {
            float maxDistance = teleportDistance > 0f ? teleportDistance : detectRange;
            if (distToPlayer > maxDistance)
            {
                TeleportToPlayer();
                attackScript?.ClearTarget();
                targetEnemy = null;
                checkTimer = 0f;
                return;
            }
        }

        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            targetEnemy = FindNearestEnemy();
            checkTimer = checkInterval;
        }

        if (targetEnemy != null && !targetEnemy.GetComponent<EnemyStats>().isDead)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance > stopDistanceToEnemy)
            {
                nav.isStopped = false;
                nav.SetDestination(targetEnemy.transform.position);
                anim.SetBool("running", true);
            }
            else
            {
                nav.isStopped = true;
                anim.SetBool("running", false);
                attackScript.SetTarget(targetEnemy); //Gửi target cho MinionAttack
                RotateTowards(targetEnemy.transform.position);
            }
        }
        else
        {
            //float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distToPlayer > followPlayerDistance)
            {
                nav.isStopped = false;
                nav.SetDestination(player.transform.position);
                anim.SetBool("running", true);
            }
            else
            {
                nav.isStopped = true;
                anim.SetBool("running", false);
                RotateTowards(player.transform.position);
            }

            attackScript.ClearTarget(); // Không có enemy thì clear target attack
        }
    }

    GameObject FindNearestEnemy()
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
                if (dist < minDist && dist <= detectRange)
                {
                    minDist = dist;
                    closest = enemy;
                }
            }
        }

        return closest;
    }

    void RotateTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }


    void TeleportToPlayer()
    {
        if (player == null) return;

        // Mặc định: đúng vị trí player (offset = 0)
        Vector3 targetPos = player.transform.position;

        // Nếu muốn tránh chồng hình, cho offset nhỏ quanh player
        if (teleportOffset > 0f)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * teleportOffset;
            targetPos += new Vector3(circle.x, 0f, circle.y);
        }

        // Ưu tiên lấy điểm hợp lệ trên NavMesh
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            nav.Warp(hit.position);
        }
        else
        {
            // Fallback: thử vài điểm quanh player
            bool warped = false;
            for (int i = 0; i < 6; i++)
            {
                float r = Mathf.Max(teleportOffset, 1.5f);
                Vector2 circle = Random.insideUnitCircle.normalized * r;
                Vector3 tryPos = player.transform.position + new Vector3(circle.x, 0f, circle.y);

                if (NavMesh.SamplePosition(tryPos, out hit, 2f, NavMesh.AllAreas))
                {
                    nav.Warp(hit.position);
                    warped = true;
                    break;
                }
            }
            if (!warped)
            {
                // Nếu vẫn không có NavMesh (ví dụ player đang ngoài NavMesh), set tạm vị trí thô
                transform.position = targetPos;
                nav.Warp(transform.position); // đồng bộ agent nội bộ
            }
        }

        // Xoay cùng hướng player (tuỳ thích)
        transform.rotation = Quaternion.LookRotation(player.transform.forward);

        // VFX nếu có
        if (teleportVfx != null)
        {
            var v = Instantiate(teleportVfx, transform.position, Quaternion.identity);
            Destroy(v, 2f);
        }

        // Reset path & animation
        nav.ResetPath();
        anim.SetBool("running", false);
    }
}
