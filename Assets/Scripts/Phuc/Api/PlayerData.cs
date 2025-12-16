using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    [Header("Basic Info")]
    public int classId;
    public int level = 1;
    public bool isNewGame = true;

    [Header("Position")]
    public float positionX;
    public float positionY;
    public float positionZ;

    [Header("Health & Mana")]
    public int currentHealth;
    public int maxHealth = 100;
    public int currentMana;
    public int maxMana = 50;

    [Header("Stats")]
    public int experience;
    public int maxArmor;
    public int maxPhysicalDamage;
    public int maxMagicDamage;
    public int maxCooldownReduction;
    public int maxMoveSpeed;
    public int maxAttackSpeed;
    public int maxAttackRange;

    [Header("Economy")]
    public int coin;

    [Header("Scene")]
    public string sceneName;

    public event Action<int> OnCoinChanged;

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
    }

    // =========================
    // COIN
    // =========================
    public void AddCoin(int amount)
    {
        coin += amount;
        OnCoinChanged?.Invoke(coin);
    }

    public bool RemoveCoin(int amount)
    {
        if (coin < amount) return false;

        coin -= amount;
        OnCoinChanged?.Invoke(coin);
        return true;
    }

    // =========================
    // SAVE POSITION (LOCAL)
    // =========================
    public void SavePlayerTransform(Transform player)
    {
        if (player == null) return;

        positionX = player.position.x;
        positionY = player.position.y;
        positionZ = player.position.z;
    }

    // =========================
    // LOAD POSITION (LOCAL)
    // =========================
    public void ApplyPlayerTransform(Transform player)
    {
        if (player == null) return;

        player.position = new Vector3(positionX, positionY, positionZ);
    }

    // =========================
    // SAVE SCENE NAME
    // =========================
    public void SaveScene()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    // =========================
    // NEW GAME RESET
    // =========================
    public void ResetForNewGame()
    {
        level = 1;
        experience = 0;
        coin = 0;

        currentHealth = maxHealth;
        currentMana = maxMana;

        positionX = positionY = positionZ = 0;
        isNewGame = true;
    }

    // =========================
    // LOAD CLASS
    // =========================
    public void LoadClass()
    {
        if (ClassDataPlayerChoose.instance == null) return;

        switch (classId)
        {
            case 1:
                ClassDataPlayerChoose.instance.ChangeClass(
                    ClassDataPlayerChoose.CharacterClass.Knight);
                break;
            case 2:
                ClassDataPlayerChoose.instance.ChangeClass(
                    ClassDataPlayerChoose.CharacterClass.Archer);
                break;
            case 3:
                ClassDataPlayerChoose.instance.ChangeClass(
                    ClassDataPlayerChoose.CharacterClass.Mage);
                break;
        }
    }
}
