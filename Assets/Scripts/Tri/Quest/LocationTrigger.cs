using System.Collections.Generic;
using UnityEngine;

public class MissionTargetLocator : MonoBehaviour
{
    public static MissionTargetLocator Instance;

    private Dictionary<string, Transform> targets = new Dictionary<string, Transform>();

    void Awake()
    {
        Instance = this;

        var points = GameObject.FindGameObjectsWithTag("MissionTarget");
        foreach (var point in points)
        {
            targets[point.name] = point.transform;
        }
    }

    public Transform GetTarget(string name)
    {
        if (targets.TryGetValue(name, out var t))
            return t;

        Debug.LogWarning($"Không tìm thấy điểm nhiệm vụ: {name}");
        return null;
    }
}
