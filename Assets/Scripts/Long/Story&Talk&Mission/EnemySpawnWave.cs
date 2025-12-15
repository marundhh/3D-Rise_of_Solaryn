using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnWave : MonoBehaviour
{
    [System.Serializable]
    public class SpawnAction
    {
        public GameObject prefab;
        public Vector3 position;
        public int count;
    }

    private Queue<SpawnAction> spawnQueue = new Queue<SpawnAction>();
    private bool isSpawning = false;

    public float spawnRadius = 5f;       // Bán kính = 5 => đường kính 10
    public float spawnInterval = 1f;     // Mỗi giây spawn 1 con

    // Hàm công khai có thể gọi từ bất cứ đâu
    public void EnqueueSpawn(GameObject prefab, Vector3 position, int count)
    {
        spawnQueue.Enqueue(new SpawnAction
        {
            prefab = prefab,
            position = position,
            count = count
        });

        if (!isSpawning)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        isSpawning = true;

        while (spawnQueue.Count > 0)
        {
            var action = spawnQueue.Dequeue();
            for (int i = 0; i < action.count; i++)
            {
                Vector3 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = action.position + new Vector3(offset.x, 0, offset.y);
                Instantiate(action.prefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        isSpawning = false;
    }
}
