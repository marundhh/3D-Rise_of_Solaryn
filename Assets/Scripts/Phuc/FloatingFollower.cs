using UnityEngine;

public class FloatingFollower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(-0.5f, 1.5f, -1f);
    public float followSpeed = 100f;
    public float maxDistance = 1f;

    private void Update()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        float distance = Vector3.Distance(transform.position, desiredPosition);

        float speed = (distance > maxDistance) ? followSpeed * 2f : followSpeed;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * speed);
    }
}
