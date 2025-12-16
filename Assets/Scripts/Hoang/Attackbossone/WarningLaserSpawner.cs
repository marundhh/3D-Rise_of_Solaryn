using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLaserSpawner : MonoBehaviour
{
    [Header("Spawner Box")]
    public BoxCollider spawnArea; // Box Collider vùng chiến đấu

    [Header("Spawn Settings")]
    public int spawnCount = 2; // số lượng spawn mỗi đợt

    [Header("Prefabs")]
    public GameObject buffaloPrefab;

    [Header("Settings")]
    public float warningTime = 2f;
    public float repeatInterval = 3f;
    public float laserWidth = 0.1f;
    public Color laserColor = Color.red;
    public bool autoStart = true;

    private List<Transform> spawners = new List<Transform>();

    private void Start()
    {
        if (!ValidateSetup()) return;

        CreateWallSpawners();

        if (autoStart)
            InvokeRepeating(nameof(SpawnFromRandomSpawner), 1f, repeatInterval);
    }

    bool ValidateSetup()
    {
        if (spawnArea == null)
        {
            Debug.LogError("[WarningLaserSpawner] Chưa gán spawnArea!");
            enabled = false;
            return false;
        }
        if (buffaloPrefab == null)
        {
            Debug.LogError("[WarningLaserSpawner] Chưa gán buffaloPrefab!");
            enabled = false;
            return false;
        }
        return true;
    }

    void CreateWallSpawners()
    {
        Bounds b = spawnArea.bounds;

        spawners.Add(CreateSpawner("NorthSpawner",
            new Vector3(b.center.x, b.center.y, b.max.z),
            Vector3.back));

        spawners.Add(CreateSpawner("SouthSpawner",
            new Vector3(b.center.x, b.center.y, b.min.z),
            Vector3.forward));

        spawners.Add(CreateSpawner("EastSpawner",
            new Vector3(b.max.x, b.center.y, b.center.z),
            Vector3.left));

        spawners.Add(CreateSpawner("WestSpawner",
            new Vector3(b.min.x, b.center.y, b.center.z),
            Vector3.right));
    }

    Transform CreateSpawner(string name, Vector3 pos, Vector3 forward)
    {
        GameObject go = new GameObject(name);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.LookRotation(forward);
        go.transform.parent = transform;
        return go.transform;
    }

    Transform FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }

        return nearest;
    }

    void SpawnFromRandomSpawner()
    {
        Transform player = FindNearestPlayer();
        if (player == null) return;

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawner = spawners[Random.Range(0, spawners.Count)];
            Vector3 moveDir = spawner.forward;

            Vector3 startPoint = GetPointOnWall(spawner, moveDir);
            Vector3 endPoint = GetOppositeWallPoint(spawner, moveDir);

            if (Mathf.Abs(moveDir.x) > 0.5f)
            {
                startPoint.z = player.position.z;
                endPoint.z = player.position.z;
            }
            else
            {
                startPoint.x = player.position.x;
                endPoint.x = player.position.x;
            }

            StartCoroutine(ShowWarningThenSpawn(startPoint, endPoint, moveDir));
        }
    }

    Vector3 GetPointOnWall(Transform spawner, Vector3 dir)
    {
        Bounds b = spawnArea.bounds;
        if (dir == Vector3.back) return new Vector3(spawner.position.x, spawner.position.y, b.max.z);
        if (dir == Vector3.forward) return new Vector3(spawner.position.x, spawner.position.y, b.min.z);
        if (dir == Vector3.left) return new Vector3(b.max.x, spawner.position.y, spawner.position.z);
        return new Vector3(b.min.x, spawner.position.y, spawner.position.z);
    }

    Vector3 GetOppositeWallPoint(Transform spawner, Vector3 dir)
    {
        Bounds b = spawnArea.bounds;
        if (dir == Vector3.back) return new Vector3(spawner.position.x, spawner.position.y, b.min.z);
        if (dir == Vector3.forward) return new Vector3(spawner.position.x, spawner.position.y, b.max.z);
        if (dir == Vector3.left) return new Vector3(b.min.x, spawner.position.y, spawner.position.z);
        return new Vector3(b.max.x, spawner.position.y, spawner.position.z);
    }

    IEnumerator ShowWarningThenSpawn(Vector3 startPoint, Vector3 endPoint, Vector3 moveDir)
    {
        GameObject warning = CreateWarningLine(startPoint, endPoint);

        yield return new WaitForSeconds(warningTime);

        Destroy(warning);

        GameObject buffalo = Instantiate(buffaloPrefab, startPoint, Quaternion.LookRotation(moveDir));
        BuffaloAI ai = buffalo.GetComponent<BuffaloAI>();
        if (ai != null) ai.SetMoveDirection(moveDir.normalized);
    }

    GameObject CreateWarningLine(Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject("WarningLine");
        LineRenderer lr = go.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = laserColor;
        lr.endColor = laserColor;
        lr.startWidth = laserWidth;
        lr.endWidth = laserWidth;
        lr.useWorldSpace = true;

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return go;
    }
}