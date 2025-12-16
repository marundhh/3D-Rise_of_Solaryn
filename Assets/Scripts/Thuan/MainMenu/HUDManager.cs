using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;
    public GameObject hudPanel;
    public Slider healthSlider;
    public Slider manaSlider;

    [Header("Level/EXP UI")]
    public Slider expSlider;                
    public TextMeshProUGUI levelText;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    void Start()
    {
        if (PlayerStats.instance != null)
        {
            healthSlider.maxValue = PlayerStats.instance.currentHealth;
            healthSlider.value = PlayerStats.instance.currentHealth;

            manaSlider.maxValue = PlayerStats.instance.currentMana;
            manaSlider.value = PlayerStats.instance.currentMana;
        }

        if (PlayerLevel.instance != null)
        {
            expSlider.maxValue = PlayerLevel.instance.expToNext;
            expSlider.value = PlayerLevel.instance.exp;
            if (levelText != null)
                levelText.text = PlayerLevel.instance.level.ToString();
        }
    }

    void Update()
    {
        if (PlayerStats.instance != null)
        {
            healthSlider.value = PlayerStats.instance.currentHealth;
            manaSlider.value = PlayerStats.instance.currentMana;
        }

        if (PlayerLevel.instance != null)
        {
            if (expSlider.maxValue != PlayerLevel.instance.expToNext)
                expSlider.maxValue = PlayerLevel.instance.expToNext;

            expSlider.value = PlayerLevel.instance.exp;

            if (levelText != null)
                levelText.text = PlayerLevel.instance.level.ToString();
        }
    }
}
