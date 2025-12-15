using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int spawnCount = 10;
    public float spawnInterval = 1f;

    [Header("Spawn Area (Box)")]
    public Vector3 boxCenter = Vector3.zero;
    public Vector3 boxSize = new Vector3(10f, 0f, 10f);

    [Header("Spawn Parent")]
    public Transform enemyHolder; // Gán GameObject trong Hierarchy ở đây

    private float spawnTimer;
    private int spawnedCount = 0;

  

    private void Update()
    {
        if (spawnedCount >= spawnCount) return;

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        Vector3 randomPosition = boxCenter + new Vector3(
            Random.Range(-boxSize.x / 2, boxSize.x / 2),
            Random.Range(-boxSize.y / 2, boxSize.y / 2),
            Random.Range(-boxSize.z / 2, boxSize.z / 2)
        );

        Instantiate(enemyPrefab, randomPosition, Quaternion.identity, enemyHolder); // Gán vào parent
        spawnedCount++;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
