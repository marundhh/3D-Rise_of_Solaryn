using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform spawnArea; // Khu vực bãi quái
    public float moveRadius = 155f; // Bán kính di chuyển
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToRandomPoint();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToRandomPoint();
        }
    }

    void MoveToRandomPoint()
    {
        if (spawnArea == null)
        {
            Debug.LogError("Spawn Area is not assigned!", this);
            return;
        }

        Vector3 randomPoint;
        if (GetRandomPoint(spawnArea.position, moveRadius, out randomPoint))
        {
            agent.SetDestination(randomPoint);
        }
    }

    bool GetRandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++) // Lặp tối đa 30 lần để tìm vị trí hợp lệ
        {
            Vector3 randomPos = center + Random.insideUnitSphere * range;
            randomPos.y = center.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, range, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

}
