using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EnemyStats : CreatureStats
{
    [Header("---------------------------")]
    public bool isInvincible = false;
    public bool isDead = false;
    public GameObject ten;
    public int expReward;
    public float damage => maxPhysicalDamage;
    public float MaxHealth => maxHealth;

    [Range(0, 100)]
    public float dodgeChance = 25f;

    public Image healthBar;

    [Header("Drop Settings")]
    public GameObject itemDropPrefab;
    public int dropCount = 1;
    [Header("Dodge Text")]
    public GameObject dodgeTextPrefab;


    [Header("Damage Text")]
    public GameObject floatingTextPrefab;       // Prefab chứa TextMeshPro
    public Transform textSpawnPoint;            // Vị trí xuất hiện text (có thể là đầu enemy)

    private void Awake()
    {
        base.Awake();

        if (healthBar == null)
            healthBar = GetComponentInChildren<Image>();

        damagepysenemy weapon = GetComponentInChildren<damagepysenemy>();
        if (weapon != null)
            weapon.Initialize(this);

        AssignWeaponDamageScript();
    }

    private void AssignWeaponDamageScript()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Weapon"))
            {
                damagepysenemy dmg = child.GetComponent<damagepysenemy>();
                if (dmg != null)
                {
                    dmg.Initialize(this);
                }
            }
        }
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.fillAmount = 1f;
    }

    public override void TakeDamage(float damage)
    {
        if (isInvincible || isDead) return;

        // Né đòn
        float roll = Random.Range(0f, 100f);
        if (roll < dodgeChance)
        {
            Debug.Log($"{gameObject.name} đã né được đòn tấn công!");
            ShowDodgeText(); // Gọi script riêng
            return;
        }

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;

        Animator hitAnim = GetComponent<Animator>();
        if (hitAnim != null)
            hitAnim.SetTrigger("hit");

        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        // Hiển thị Damage Text
        ShowFloatingText(damage);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
            GameEventSystem.Dispatch(new EnemyKilledEvent(GetComponent<EnemyNameManager>().enemyData.displayName));
        }



    }
    private void ShowDodgeText()
    {
        if (dodgeTextPrefab != null)
        {
            Vector3 spawnPos = textSpawnPoint != null ? textSpawnPoint.position : transform.position + Vector3.up * 2f;
            GameObject dodgeText = Instantiate(dodgeTextPrefab, spawnPos, Quaternion.identity);
            dodgeText.GetComponent<DodgeText>().Setup();
        }
    }

    private void ShowFloatingText(string message)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = textSpawnPoint != null ? textSpawnPoint.position : transform.position + Vector3.up * 2f;
            GameObject floatingText = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            floatingText.GetComponent<FloatingText>().Setup(message);
        }
    }

    private void ShowFloatingText(float damage)
    {
        ShowFloatingText(damage.ToString("F0"));
    }


    protected override void Die()
    {
        isDead = true;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.applyRootMotion = false;
            anim.SetTrigger("death");
            anim.SetBool("running", false);
        }

        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        if (nav != null)
        {
            nav.ResetPath();
            nav.velocity = Vector3.zero;
            nav.isStopped = true;
            nav.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null)
            enemyAttack.enabled = false;

        AI_Movement ai = GetComponent<AI_Movement>();
        if (ai != null)
            ai.enabled = false;

        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        SoulManager.Instance?.StoreSoul(this);

        //fix chổ này cho t nè lổi đó 
        PlayerLevel.instance.GainExp(expReward);

        Invoke(nameof(DropItems), 7f);

        Destroy(gameObject, 7f);
        if (ten != null)
            Destroy(ten);
   
    }


    private void DropItems()
    {
        if (itemDropPrefab == null) return;

        for (int i = 0; i < dropCount; i++)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        }
    }
}