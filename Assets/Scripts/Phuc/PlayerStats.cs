using System;
using UnityEngine;

public class PlayerStats : CreatureStats
{
    public static PlayerStats instance;

    [Header("---------------------------")]

    [Header("Class Manager")]
    public ClassManager classManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        base.Awake();
        InitStats();
    }

    private void InitStats()
    {
        currentAttackRange = classManager.selectedClassData.attackRange;
    }

    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        Debug.Log("Player đã chết.");
        // Có thể thêm logic load màn thua, animation chết, v.v...
    }

}
