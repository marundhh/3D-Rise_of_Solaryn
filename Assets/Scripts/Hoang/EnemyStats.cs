using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyStats : CreatureStats
{
    [Header("---------------------------")]
    public bool isInvincible = false;
    public bool isDead = false;
    public GameObject ten;
    public float damage => maxPhysicalDamage;
    
    public float MaxHealth => maxHealth;


    [Range(0, 100)]
    public float dodgeChance = 25f; // Tỷ lệ né tránh (%, từ 0 đến 100)


    public Image healthBar;


    [Header("Drop Settings")]
    public GameObject itemDropPrefab;
    public int dropCount = 1;

  

    private void Awake()
    {
        base.Awake(); // Gọi Awake của CreatureStats để gán currentHealth

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

        // Tính xác suất né
        float roll = Random.Range(0f, 100f);
        if (roll < dodgeChance)
        {
            Debug.Log($"{gameObject.name} đã né được đòn tấn công!");

            Animator anim = GetComponent<Animator>();
    
           
            return; // Không nhận sát thương nếu né được
        }

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;

        Animator hitAnim = GetComponent<Animator>();
        if (hitAnim != null)
            hitAnim.SetTrigger("hit");

        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
            GameEventSystem.Dispatch(new EnemyKilledEvent(GetComponent<EnemyNameManager>().enemyData.displayName));
        }
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

        // Tắt các script điều khiển
        EnemyAttack enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack != null)
            enemyAttack.enabled = false;

        AI_Movement ai = GetComponent<AI_Movement>();
        if (ai != null)
            ai.enabled = false;

        // Tắt mọi script trừ EnemyStats
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        DropItems();

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
