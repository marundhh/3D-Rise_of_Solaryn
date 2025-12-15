using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnArea;
    public int maxEnemies = 5;
    public float spawnInterval = 3f;
    private int currentEnemies = 0;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies) return;

        Vector3 spawnPosition = spawnArea.position + Random.insideUnitSphere * 3f;
        spawnPosition.y = spawnArea.position.y;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        currentEnemies++;
    }
}
