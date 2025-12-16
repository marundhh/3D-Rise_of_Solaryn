using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class PlayerStats : CreatureStats
{
    public static PlayerStats instance;
    public CameraController cameraController; // CameraController (để điều khiển camera)

    [Header("Refs")]
    [Tooltip("Child có tag Player (chứa PlayerMove.cs)")]
    public Transform playerRoot;          // PlayerRoot (child, tag=Player)
    [Tooltip("Cha ngoài cùng của cả rig")]
    public Transform playerTop;           // Player (parent ngoài cùng)
    public Vector3 spawnWorldPos;         // Vị trí spawn (world)
    public Vector3 spawnWorldEuler;       // Hướng nhìn spawn (tùy chọn)

    [Header("Class Manager")]
    public ClassManager classManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        LoadStatsFromPlayerData();
    }

    void Start()
    {
        SetupForRespawn(); 
        InitStats();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            Die();
            Debug.Log("Stats reset to default values.");
        }
    }
    public void SetupForRespawn()
    {
        CacheRefs();
        // Lưu spawn theo thế giới từ cha ngoài cùng
        spawnWorldPos = playerTop.position;
        spawnWorldEuler = playerTop.eulerAngles;
    }
    void CacheRefs()
    {
        if (playerRoot == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            playerRoot = go ? go.transform : null;        // child
        }
        if (playerTop == null && playerRoot != null)
        {
            playerTop = playerRoot.parent != null ? playerRoot.parent : playerRoot; // cha ngoài cùng
        }
    }

    void InitStats()
    {
        maxAttackRange = classManager.selectedClassData.attackRange;
        currentAttackRange = classManager.selectedClassData.attackRange;
    }

    public override void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0) Die();
    }

    protected override void Die()
    {
        PlayerDeathUI.Instance.ShowDeathScreen();
        ResetStats();
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f); // đợi 1 giây trước khi respawn
        CacheRefs();

        // 1) TẮT điều khiển chuyển động/physics để tránh ghi đè
        var move = playerRoot ? playerRoot.GetComponent<PlayerMovement>() : null;
        var cc = playerRoot ? playerRoot.GetComponent<CharacterController>() : null;
        var rb = (playerTop ? playerTop.GetComponent<Rigidbody>() : null) ?? (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null);
        var anim = playerTop ? playerTop.GetComponentInChildren<Animator>() : null;

        if (move) move.enabled = false;
        if (cc) cc.enabled = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // tạm thời để teleport sạch
        }
        if (anim) anim.applyRootMotion = false;

        // 2) DỊCH CHUYỂN: cha về spawn (world), con về gốc (local)
        if (playerTop) { playerTop.position = spawnWorldPos; playerTop.rotation = Quaternion.Euler(spawnWorldEuler); }
        if (playerRoot) { playerRoot.localPosition = Vector3.zero; playerRoot.localRotation = Quaternion.identity; }

        Physics.SyncTransforms(); // đồng bộ ngay lập tức
        yield return null;        // đợi qua 1 frame, tránh script khác ghi đè trong cùng frame

        // 3) BẬT LẠI theo thứ tự
        if (rb) rb.isKinematic = false;
        if (cc) cc.enabled = true;
        if (move) move.enabled = true;
        if (anim) anim.applyRootMotion = true;

        Debug.Log($"[Respawn OK] Top(world)={playerTop.position} | Root(local)={playerRoot.localPosition}");
    }

    public void ResetStats()
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
    private void LoadStatsFromPlayerData()
    {
        PlayerData data = PlayerData.instance;

        if (data.maxHealth <= 0)
        {
            base.Awake();
            return;
        }

        maxHealth = data.maxHealth;
        maxArmor = data.maxArmor;
        maxMana = data.maxMana;
        maxPhysicalDamage = data.maxPhysicalDamage;
        maxMagicDamage = data.maxMagicDamage;
        maxCooldownReduction = data.maxCooldownReduction;
        maxMoveSpeed = data.maxMoveSpeed;
        maxAttackSpeed = data.maxAttackSpeed;
        maxAttackRange = data.maxAttackRange;

        currentHealth = data.currentHealth;
        currentArmor = data.maxArmor;
        currentMana = data.currentMana;
        currentPhysicalDamage = data.maxPhysicalDamage;
        currentMagicDamage = data.maxMagicDamage;
        currentCooldownReduction = data.maxCooldownReduction;
        currentMoveSpeed = data.maxMoveSpeed;
        currentAttackSpeed = data.maxAttackSpeed;
        currentAttackRange = data.maxAttackRange;
    }

    #region Stat Upgrades
    public void AddHealthPercent(float percent)
    {
        float bonus = maxHealth * (percent / 100f);
        maxHealth += bonus;
        currentHealth += bonus;
    }

    public void AddArmorPercent(float percent)
    {
        float bonus = maxArmor * (percent / 100f);
        maxArmor += bonus;
        currentArmor += bonus;
    }

    public void AddManaPercent(float percent)
    {
        float bonus = maxMana * (percent / 100f);
        maxMana += bonus;
        currentMana += bonus;
    }

    public void AddPhysicalDamagePercent(float percent)
    {
        float bonus = maxPhysicalDamage * (percent / 100f);
        maxPhysicalDamage += bonus;
        currentPhysicalDamage += bonus;
    }

    public void AddMagicDamagePercent(float percent)
    {
        float bonus = maxMagicDamage * (percent / 100f);
        maxMagicDamage += bonus;
        currentMagicDamage += bonus;
    }

    public void AddCooldownReductionPercent(float percent)
    {
        float bonus = (percent / 100f);
        maxCooldownReduction = Mathf.Clamp(maxCooldownReduction + bonus, 0f, 0.6f); ;
        currentCooldownReduction = Mathf.Clamp(maxCooldownReduction + bonus, 0f, 0.6f); ;
    }
    public void AddMoveSpeedPercent(float percent)
    {
        float bonus = maxMoveSpeed * (percent / 100f);
        maxMoveSpeed += bonus;
        currentMoveSpeed += bonus;
    }

    public void AddAttackSpeedPercent(float percent)
    {
        float bonus = maxAttackSpeed * (percent / 100f);
        maxAttackSpeed += bonus;
        currentAttackSpeed += bonus;
    }

    public void AddAttackRangePercent(float percent)
    {
        float bonus = maxAttackRange * (percent / 100f);
        maxAttackRange += bonus;
        currentAttackRange += bonus;
    }
    #endregion
}
