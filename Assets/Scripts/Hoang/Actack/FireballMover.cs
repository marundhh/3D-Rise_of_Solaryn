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

    public Transform target; // Player hoặc Minion
    public float trackingSpeed = 10f;
    public float rotateSpeed = 5f;
    public float hitDistance = 0.5f;

    private EnemyStats caster;
    public float damage => caster != null ? caster.damage : 0f;

    public void Initialize(Vector3 start, Vector3 v0, Vector3 gravityVec, float timeToTarget, EnemyStats owner, Transform forcedTarget = null)
    {
        startPos = start;
        velocity = v0;
        gravity = gravityVec;
        totalTime = timeToTarget;
        caster = owner;
        target = forcedTarget;
    }

    void Update()
    {
        if (target == null)
        {
            // Tìm mục tiêu gần nhất giữa Player và Minion
            target = FindNearestTarget(new string[] { "Player", "Minion" });
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }
        }

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

            transform.forward = velocity + gravity * elapsed;

            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance <= hitDistance)
                {
                    AttackTarget(target);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void TrackingMove()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * trackingSpeed * Time.deltaTime;

        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotateSpeed * Time.deltaTime);

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= hitDistance)
        {
            AttackTarget(target);
            Destroy(gameObject);
        }
    }

    void AttackTarget(Transform tgt)
    {
        if (tgt.CompareTag("Player"))
        {
            if (PlayerStats.instance == null) return;
            PlayerStats.instance.TakeDamage(damage);
            Debug.Log($"Fireball gây {damage} sát thương lên Player.");
        }
        else if (tgt.CompareTag("Minion"))
        {
            MinionStats minionStats = tgt.GetComponent<MinionStats>();
            if (minionStats != null)
            {
                minionStats.TakeDamage(damage);
                Debug.Log($"Fireball gây {damage} sát thương lên Minion.");
            }
        }
    }

    Transform FindNearestTarget(string[] tags)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (string tag in tags)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in objs)
            {
                float dist = Vector3.Distance(transform.position, obj.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = obj.transform;
                }
            }
        }

        return nearest;
    }
}
