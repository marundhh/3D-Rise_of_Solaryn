using UnityEngine;

public class meteordamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;

    private EnemyStats boss;

    private void Start()
    {
        // 🔍 Tìm GameObject boss theo tên trong Hierarchy
        GameObject bossObj = GameObject.Find("REAPER_LEGACY");

        if (bossObj != null)
        {
            boss = bossObj.GetComponent<EnemyStats>();
        }
        else
        {
            Debug.LogWarning("⚠️ Không tìm thấy Boss theo tên 'REAPER_LEGACY'. Kiểm tra lại trong Hierarchy.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Gây damage cho Player
        if (other.CompareTag("Player"))
        {
            if (PlayerStats.instance == null) return;

            PlayerStats.instance.TakeDamage(damage);
            Debug.Log($"🌩️ Meteor gây {damage} damage lên Player.");

            HealBoss(damage);
        }

        // Gây damage cho Minion
        if (other.CompareTag("Minion"))
        {
            MinionStats minionStats = other.GetComponent<MinionStats>();
            if (minionStats != null)
            {
                minionStats.TakeDamage(damage);
                Debug.Log($"🌩️ Meteor gây {damage} damage lên Minion.");
                HealBoss(damage);
            }
        }
    }

    /// <summary>
    /// Hồi máu cho Boss khi Meteor trúng mục tiêu
    /// </summary>
    private void HealBoss(float healAmount)
    {
        if (boss != null)
        {
            boss.currentHealth += healAmount;
            boss.currentHealth = Mathf.Min(boss.currentHealth, boss.MaxHealth);

            if (boss.healthBar != null)
                boss.healthBar.fillAmount = boss.currentHealth / boss.MaxHealth;

            Debug.Log($"💖 Boss hồi {healAmount} máu (hiện tại: {boss.currentHealth}/{boss.MaxHealth}).");
        }
    }
}
