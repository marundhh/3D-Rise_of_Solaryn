using UnityEngine;

public class SpawnBossTrigger : MonoBehaviour
{
    public GameObject bossPrefab;            // Prefab của boss
    public Transform spawnPoint;             // Vị trí spawn boss
    public GameObject[] objectsToDestroy;    // Các object cần xóa
    public GameObject[] objectsToActivate;   // Các object cần hiện ra

    private bool hasTriggered = false;       // Ngăn kích hoạt nhiều lần

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            SpawnBoss();
            DestroyOldObjects();
            ActivateNewObjects();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab != null && spawnPoint != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Boss Prefab hoặc Spawn Point chưa được gán.");
        }
    }

    private void DestroyOldObjects()
    {
        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    private void ActivateNewObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
