using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class MeteorSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject indicatorPrefab;
    public GameObject meteorPrefab;

    [Header("Spawn Settings")]
    public float delayBeforeMeteorFall = 1.5f;
    public float meteorHeight = 30f;
    public float spawnInterval = 2f;
    public int meteorsPerSpawn = 5;
    public float meteorFallSpeed = 20f;
    public float delayBetweenEachMeteor = 0.2f;

    [Header("Ground Settings")]
    public float groundY = 0f; // Mặc định là 0, sẽ tự động lấy từ mặt đất nếu có collider
    public bool autoDetectGroundY = true; // ✅ Tự động lấy Y từ collider mặt đất

    private BoxCollider spawnArea;

    private void Start()
    {
        spawnArea = GetComponent<BoxCollider>();

        if (spawnArea == null)
        {
            Debug.LogError("❌ Không tìm thấy BoxCollider trong GameObject Spawner!");
            return;
        }

        if (autoDetectGroundY)
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 100f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 200f))
            {
                if (hit.collider.CompareTag("Ground")) // ✅ Kiểm tra tag Ground
                {
                    groundY = hit.point.y;
                    Debug.Log("✅ Ground Y tự động phát hiện là: " + groundY);
                }
                else
                {
                    Debug.LogWarning("⚠️ Raycast trúng vật thể không có tag 'Ground', dùng giá trị groundY mặc định.");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Không tìm thấy mặt đất, dùng giá trị groundY mặc định.");
            }
        }


        InvokeRepeating(nameof(StartSpawnWave), 1f, spawnInterval);
    }

    void StartSpawnWave()
    {
        StartCoroutine(SpawnMeteorWave());
    }

    IEnumerator SpawnMeteorWave()
    {
        for (int i = 0; i < meteorsPerSpawn; i++)
        {
            Vector3 localCenter = spawnArea.center;
            Vector3 localSize = spawnArea.size;

            // Tọa độ XZ ngẫu nhiên trong vùng spawn
            float x = Random.Range(-localSize.x / 2f, localSize.x / 2f);
            float z = Random.Range(-localSize.z / 2f, localSize.z / 2f);
            Vector3 localRandom = new Vector3(x, 0f, z);
            Vector3 worldRandomPos = transform.TransformPoint(localCenter + localRandom);

            // Gán vị trí mặt đất Y
            Vector3 groundPos = new Vector3(worldRandomPos.x, groundY, worldRandomPos.z);

            // Spawn indicator
            GameObject indicator = Instantiate(indicatorPrefab, groundPos, Quaternion.identity);
            Destroy(indicator, delayBeforeMeteorFall + 2f);

            // Spawn meteor sau delay
            StartCoroutine(SpawnMeteorAfterDelay(groundPos, delayBeforeMeteorFall));

            yield return new WaitForSeconds(delayBetweenEachMeteor);
        }
    }

    IEnumerator SpawnMeteorAfterDelay(Vector3 targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnMeteor(targetPosition);
    }

    void SpawnMeteor(Vector3 targetPosition)
    {
        Vector3 spawnPos = targetPosition + Vector3.up * meteorHeight;
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = meteor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.down * meteorFallSpeed;
        }
        else
        {
            Debug.LogWarning("⚠️ Meteor prefab thiếu Rigidbody! Không thể gán tốc độ rơi.");
        }
    }
}
