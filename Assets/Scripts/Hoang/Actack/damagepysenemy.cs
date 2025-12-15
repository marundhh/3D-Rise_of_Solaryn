using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagepysenemy : MonoBehaviour
{
    private EnemyStats caster; // Enemy chủ sở hữu vũ khí

    public GameObject bloodEffectPrefab;
    public AudioClip hitSound;
    private AudioSource audioSource;

    public float damage => caster != null ? caster.damage : 0f; // fallback nếu null

    public void Initialize(EnemyStats owner)
    {
        caster = owner;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.instance == null) return;

            PlayerStats.instance.TakeDamage(damage);
            Debug.Log($"Enemy gây {damage} sát thương lên Player.");

            // Phát âm thanh trúng đòn
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Hiệu ứng máu văng
            if (bloodEffectPrefab != null)
            {
                Vector3 spawnPos = other.ClosestPoint(transform.position); // vị trí gần nhất giữa enemy và player
                GameObject blood = Instantiate(bloodEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(blood, 2f); // Tự hủy sau 2 giây
            }
        }
    }


}
