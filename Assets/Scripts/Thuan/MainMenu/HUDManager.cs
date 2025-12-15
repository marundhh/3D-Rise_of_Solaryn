using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;

    void Start()
    {
        if (PlayerStats.instance != null)
        {
            healthSlider.maxValue = PlayerStats.instance.currentHealth;
            healthSlider.value = PlayerStats.instance.currentHealth;

            manaSlider.maxValue = PlayerStats.instance.currentMana;
            manaSlider.value = PlayerStats.instance.currentMana;
        }
        
    }

    void Update()
    {
        if (PlayerStats.instance != null)
        {
            healthSlider.value = PlayerStats.instance.currentHealth;
            manaSlider.value = PlayerStats.instance.currentMana;
        }
    }
}
