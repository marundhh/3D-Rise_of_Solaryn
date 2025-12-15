using UnityEngine;

public class VisitLocationChecker : MonoBehaviour
{
    public Transform player;       // Drag từ Player
    public Transform targetPoint;  // Điểm cần đến (checkpoint)
    public float detectRadius = 2f;
    private bool missionReported = false;

    void Update()
    {
       /* if (missionReported) return;

        float distance = Vector3.Distance(player.position, targetPoint.position);
       // Debug.Log($"[DEBUG] Khoảng cách tới mục tiêu: {distance}");

        if (distance <= detectRadius)
        {
            GameEventSystem.Dispatch(new ReachedLocationEvent("WestGate"));

            missionReported = true;
           // Debug.Log("[DEBUG] Đã đến đúng vị trí → gửi báo cáo nhiệm vụ");
            //Debug.Log("[TEST] VisitLocationChecker đang chạy!");
        }*/
    }

}
