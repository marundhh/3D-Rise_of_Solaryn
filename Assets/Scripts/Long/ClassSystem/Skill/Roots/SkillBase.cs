using UnityEngine;

public abstract class SkillBase : ScriptableObject
{
    public string SkillName { get { return skillName; } }
    public string SkillDescription { get { return skillDescription; } }
    public float Cooldown { get { return cooldown; } }
    public float Duration { get { return duration; } }
    public float ManaCost { get { return manaCost; } }
    public PlayerStats playerStats { get { return playerStats; } }

    [SerializeField] private string skillName;
    [SerializeField] private string skillDescription;
    [SerializeField] private float cooldown;
    [SerializeField] private float duration;
    [SerializeField] private float manaCost;

    public AudioClip skillSoundEff;
    public GameObject skillEffect;

}