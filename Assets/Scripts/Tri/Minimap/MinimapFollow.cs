using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player;
    public float height = 20f;
    private bool isFollowing = true;

    public bool IsFollowing => isFollowing;

    void LateUpdate()
    {
        if (!isFollowing || player == null) return;

        Vector3 newPos = player.position;
        newPos.y += height;
        transform.position = newPos;
    }

    public void SetFollow(bool follow)
    {
        isFollowing = follow;
    }
}
