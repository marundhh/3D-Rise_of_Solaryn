using UnityEngine;

public class MiniMapSceneCamera : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0, 50, 0); // độ cao camera nhìn từ trên xuống

    void Start()
    {
        FindPlayer(); // Tìm player ngay khi scene load
    }

    void LateUpdate()
    {
        if (player == null)
        {
            FindPlayer(); // Nếu vì lý do nào đó mất reference thì tìm lại
            return;
        }

        // Camera bám theo player
        transform.position = player.position + offset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // góc nhìn từ trên xuống
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            Debug.LogWarning("⚠️ MiniMapSceneCamera: Không tìm thấy Player trong scene!");
        }
    }
}
