using System.Collections;
using UnityEngine;

public class EffectEnemySpawner : MonoBehaviour
{
    [Header("Hiệu ứng để kiểm tra (GameObject bật lên sẽ sinh enemy)")]
    public GameObject effectObject;

    [Header("Prefab của Enemy muốn spawn")]
    public GameObject enemyPrefab;

    [Header("Cài đặt spawn")]
    public int numberOfEnemies = 5;         // Bao nhiêu enemy muốn spawn
    public float verticalSpacing = 1.5f;    // Khoảng cách theo trục Y giữa các enemy
    public float spawnDelay = 0.2f;         // Độ trễ giữa các lần spawn

    private bool hasSpawned = false;        // Đảm bảo chỉ spawn 1 lần khi hiệu ứng bật

    void Update()
    {
        // Nếu effect được bật và chưa spawn thì bắt đầu
        if (effectObject.activeSelf && !hasSpawned)
        {
            hasSpawned = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * i * verticalSpacing;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Nếu bạn muốn reset để spawn lại mỗi lần bật lại effect
    private void OnDisable()
    {
        hasSpawned = false;
    }
}
