using UnityEngine;

[System.Serializable]
public class EnemySoulData
{
    public float health;
    public float damage;
    public float defense;
    public string enemyName;

    public EnemySoulData(float health, float damage, float defense = 0f, string enemyName = "")
    {
        this.health = health;
        this.damage = damage;
        this.defense = defense;
        this.enemyName = enemyName;
    }
}
