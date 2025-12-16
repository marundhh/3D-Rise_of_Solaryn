using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionWeaponDamage : MonoBehaviour
{
    private MinionStats owner;

    public float damage => owner != null ? owner.damage : 0f;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public AudioClip hitSound;
    private AudioSource audioSource;

    public void Initialize(MinionStats stats)
    {
        owner = stats;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            if (enemyStats != null && !enemyStats.isDead)
            {
                enemyStats.TakeDamage(damage);
                Debug.Log($"Minion gây {damage} sát thương lên {enemyStats.name}");

                // Âm thanh
                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
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
    }
}
