using UnityEngine;

public class SmoothFollowTarget : MonoBehaviour
{
    public Transform playerRoot;
    public float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (playerRoot == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, playerRoot.position, ref velocity, smoothTime);
    }
}
