using UnityEngine;

public abstract class CreatureStats : MonoBehaviour
{
    [Header("MAX STATS")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float maxArmor;
    [SerializeField] protected float maxMana;
    [SerializeField] protected float maxPhysicalDamage;
    [SerializeField] protected float maxMagicDamage;
    [SerializeField] protected float maxCooldownReduction;
    [SerializeField] protected float maxMoveSpeed;
    [SerializeField] protected float maxAttackSpeed;
    [SerializeField] protected float maxAttackRange;

    public float MaxHealth => maxHealth;
    public float MaxArmor => maxArmor;
    public float MaxMana => maxMana;
    public float MaxPhysicalDamage => maxPhysicalDamage;
    public float MaxMagicDamage => maxMagicDamage;
    public float MaxCooldownReduction => maxCooldownReduction;
    public float MaxMoveSpeed => maxMoveSpeed;
    public float MaxAttackSpeed => maxAttackSpeed;
    public float MaxAttackRange => maxAttackRange;

    [Header("---------------------------")]

    [Header("CURRENT STATS")]
    public float currentHealth;
    public float currentArmor;
    public float currentMana;
    public float currentPhysicalDamage;
    public float currentMagicDamage;
    public float currentCooldownReduction;
    public float currentMoveSpeed;
    public float currentAttackSpeed;
    public float currentAttackRange;

    protected void Awake()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
        currentMana = maxMana;
        currentPhysicalDamage = maxPhysicalDamage;
        currentMagicDamage = maxMagicDamage;
        currentCooldownReduction = maxCooldownReduction;
        currentMoveSpeed = maxMoveSpeed;
        currentAttackSpeed = maxAttackSpeed;
        currentAttackRange = maxAttackRange;
    }

    public abstract void TakeDamage(float damage);

    protected abstract void Die();
}