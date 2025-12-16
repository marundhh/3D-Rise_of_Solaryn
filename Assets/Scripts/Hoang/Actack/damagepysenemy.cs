using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damagepysenemy : MonoBehaviour
{
    private EnemyStats caster; // Enemy chủ sở hữu vũ khí

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public AudioClip hitSound;
    private AudioSource audioSource;

    [Header("Knockback Settings")]
    public float knockbackForce = 0f; // khoảng cách đẩy cho Player
    public float knockbackForceMinion = 0f; // lực đẩy Minion (dùng Rigidbody)
    [Tooltip("Góc xoay thêm quanh trục Y so với hướng enemy → target (độ)")]
    public float knockbackAngleY = 0f;

    public float damage => caster != null ? caster.damage : 0f;

    // Gán enemy chủ sở hữu
    public void Initialize(EnemyStats owner)
    {
        caster = owner;
    }

    private void Start()
    {
        // Nếu chưa có AudioSource, tự thêm
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.instance == null) return;

            PlayerStats.instance.TakeDamage(damage);
            Debug.Log($"Enemy gây {damage} sát thương lên Player.");

            SpawnEffectAndSound(other);

            // Đẩy player lùi ra xa enemy (CharacterController)
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                Vector3 customDir = Quaternion.Euler(0, knockbackAngleY, 0) * knockbackDir;
                customDir.y = 0;
                customDir.Normalize();

                controller.Move(customDir * knockbackForce);
            }
        }

        if (other.CompareTag("Minion"))
        {
            MinionStats minionStats = other.GetComponent<MinionStats>();
            if (minionStats != null)
            {
                minionStats.TakeDamage(damage);
                Debug.Log($"Enemy gây {damage} sát thương lên Minion.");

                SpawnEffectAndSound(other);

                // Đẩy Minion bằng Rigidbody
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
                    Vector3 customDir = Quaternion.Euler(0, knockbackAngleY, 0) * knockbackDir;
                    customDir.y = 0; // tránh đẩy lên trời
                    customDir.Normalize();

                    rb.AddForce(customDir * knockbackForceMinion, ForceMode.Impulse);
                }
            }
        }
    }

    private void SpawnEffectAndSound(Collider other)
    {
        // Phát âm thanh trúng đòn bằng PlayOneShot
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound, 1f); // âm lượng = 1
        }

        // Hiệu ứng máu
        if (bloodEffectPrefab != null)
        {
            Vector3 spawnPos = other.ClosestPoint(transform.position);
            GameObject blood = Instantiate(bloodEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(blood, 2f);
        }
    }
}
