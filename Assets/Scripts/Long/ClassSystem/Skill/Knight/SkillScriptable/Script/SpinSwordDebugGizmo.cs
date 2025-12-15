using UnityEngine;

public class SpinSwordDebugGizmo : MonoBehaviour
{
    public float radius = 2f;                // phạm vi xoay kiếm
    public Color gizmoColor = Color.red;     // màu vòng

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
