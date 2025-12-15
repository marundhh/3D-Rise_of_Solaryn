using UnityEngine;
using TMPro;

public class MissionNavigator : MonoBehaviour
{
    public Transform player;
    public RectTransform arrowUI;
    public TextMeshProUGUI distanceText;

    private ReachLocationMission currentMission;

    void Update()
    {
        if (currentMission == null || currentMission.IsCompleted) return;

        Transform target = MissionTargetLocator.Instance.GetTarget(currentMission.LocationName);
        if (target == null) return;

        // 1. Tính hướng
        Vector3 dir = (target.position - player.position).normalized;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        arrowUI.rotation = Quaternion.Euler(0, 0, -angle); // UI xoay ngược trục Z

        // 2. Tính khoảng cách
        float distance = Vector3.Distance(player.position, target.position);
        distanceText.text = $"{distance:F1}m";
    }

    public void SetMission(ReachLocationMission mission)
    {
        currentMission = mission;
    }
}
