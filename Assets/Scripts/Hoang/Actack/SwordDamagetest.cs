using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamagetest : MonoBehaviour
{
    public float damageAmount = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(PlayerStats.instance.currentPhysicalDamage);
            }
        }
    }
}
