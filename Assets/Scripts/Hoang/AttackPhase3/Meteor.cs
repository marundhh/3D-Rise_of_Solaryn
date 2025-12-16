using UnityEngine;

public class Meteor : MonoBehaviour
{
    public GameObject explosionEffect; // ✅ Prefab hiệu ứng nổ
    public float explosionEffectDuration = 1f;


    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
           
            // Tạo hiệu ứng nổ
            if (explosionEffect != null)
            {
                GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(explosion, explosionEffectDuration); // 🧹 Xoá hiệu ứng nổ sau vài giây
            }

            // Gọi rung màn hình
            // Gọi rung màn hình với Cinemachine
            CameraShakeCinemachine.Instance.Shake(5f, 0.5f);


            Destroy(gameObject); // Xoá viên đá sau khi nổ
        }
    }
}
