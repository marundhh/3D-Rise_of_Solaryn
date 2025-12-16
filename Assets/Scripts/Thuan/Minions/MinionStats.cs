using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class MinionStats : CreatureStats
{
    [Header("---------------------------")]
    public bool isInvincible = false;
    public bool isDead = false;
    public GameObject ten;
    public float damage => maxPhysicalDamage;
    public float MaxHealth => maxHealth;

    public Image healthBar;

    [Header("Damage Text")]
    public GameObject floatingTextPrefab;
    public Transform textSpawnPoint;

    private void Awake()
    {
        base.Awake();

        if (healthBar == null)
            healthBar = GetComponentInChildren<Image>();

        MinionWeaponDamage weapon = GetComponentInChildren<MinionWeaponDamage>();
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
                MinionWeaponDamage dmg = child.GetComponent<MinionWeaponDamage>();
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

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;

        Animator hitAnim = GetComponent<Animator>();
        if (hitAnim != null)
            hitAnim.SetTrigger("hit");

        Debug.Log($"{gameObject.name} nhận {damage} sát thương. Máu còn lại: {currentHealth}");

        ShowFloatingText(damage);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
            GameEventSystem.Dispatch(new EnemyKilledEvent(GetComponent<MinionNameManager>().minionData.displayName));
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;

        ShowFloatingText(amount); // Hiện số hồi máu như sát thương
    }

    public void BuffFromSoul(EnemySoulData soul, float ratio, float duration)
    {
        float addedHealth = soul.health * ratio;
        float addedDamage = soul.damage * ratio;

        maxHealth += addedHealth;
        maxPhysicalDamage += addedDamage;

        currentHealth += addedHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        currentPhysicalDamage += addedDamage;

        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;

        Debug.Log($"Minion được buff {addedHealth} máu và {addedDamage} damage từ linh hồn");
    }


    private void ShowFloatingText(string message)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = textSpawnPoint != null 
                ? textSpawnPoint.position 
                : transform.position + Vector3.up * 2f;


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

        Destroy(gameObject, 7f);
        if (ten != null)
            Destroy(ten);
    }
}