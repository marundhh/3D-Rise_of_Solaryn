using UnityEngine;

public class FireballMover : MonoBehaviour
{
    public enum MoveMode { Parabola, Tracking }
    public MoveMode moveMode = MoveMode.Parabola;

    Vector3 startPos;
    Vector3 velocity;
    Vector3 gravity;
    float totalTime;
    float elapsed = 0f;

    public Transform target; // Player
    public float trackingSpeed = 10f; // Tốc độ dí theo
    public float rotateSpeed = 5f;    // Tốc độ xoay theo hướng
    public float hitDistance = 0.5f; // Khoảng cách tối thiểu để tính là trúng player

    private EnemyStats caster; // Enemy bắn
    public float damage => caster != null ? caster.damage : 0f;

    public void Initialize(Vector3 start, Vector3 v0, Vector3 gravityVec, float timeToTarget, EnemyStats owner, Transform player = null)
    {
        startPos = start;
        velocity = v0;
        gravity = gravityVec;
        totalTime = timeToTarget;
        caster = owner; // 👈 Gán EnemyStats
        target = player;
    }


    void Update()
    {
        if (moveMode == MoveMode.Parabola)
        {
            ParabolaMove();
        }
        else if (moveMode == MoveMode.Tracking)
        {
            TrackingMove();
        }
    }

    void ParabolaMove()
    {
        if (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            transform.position = startPos + velocity * elapsed + 0.5f * gravity * elapsed * elapsed;

            // Xoay theo hướng bay
            transform.forward = velocity + gravity * elapsed;
        }
        else
        {
            Destroy(gameObject);
        }
        AttackPlayer();
    }

    void TrackingMove()
    {
        // Tự động tìm player nếu chưa có target
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * trackingSpeed * Time.deltaTime;

            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);

            // Kiểm tra khoảng cách để tự hủy
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= hitDistance)
            {
                // Có thể thêm gây sát thương tại đây nếu cần
                // target.GetComponent<PlayerHealth>().TakeDamage(damage);
                AttackPlayer();
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AttackPlayer()
    {
        if (PlayerStats.instance == null) return;

        PlayerStats.instance.TakeDamage(damage);
        Debug.Log($"Fireball gây {damage} sát thương lên Player.");
    }

}