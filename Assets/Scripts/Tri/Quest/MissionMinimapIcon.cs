using UnityEngine;

public class MissionMinimapIcon : MonoBehaviour
{
    private Transform target;
    private Transform player;

    public void Init(Transform target, Transform player)
    {
        this.target = target;
        this.player = player;
    }

    void LateUpdate()
    {
        if (target == null || player == null) return;

        // Giữ icon trên vị trí target
        Vector3 pos = target.position;
        pos.y = transform.position.y;
        transform.position = pos;

        // Quay icon về hướng player nếu cần
        Vector3 dir = player.position - target.position;
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(90, 0, -angle);
    }
}
