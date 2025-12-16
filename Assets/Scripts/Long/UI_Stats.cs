using TMPro;
using UnityEngine;

public class UI_Stats : MonoBehaviour
{
    [Header("UI Refs")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI physicalDamageText;
    public TextMeshProUGUI cooldownReductionText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI attackRangeText;

    /// <summary>
    /// Hàm này gọi khi mở bảng stats
    /// </summary>
    /// 
    private void OnEnable()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        var stats = PlayerStats.instance;
        if (stats == null) return;

        healthText.text = $"{stats.currentHealth}/{stats.MaxHealth}";
        armorText.text = $"{stats.currentArmor}/{stats.MaxArmor}";
        manaText.text = $"{stats.currentMana}/{stats.MaxMana}";
        physicalDamageText.text = $"{stats.currentPhysicalDamage}/{stats.MaxPhysicalDamage}";
        cooldownReductionText.text = $"{stats.currentCooldownReduction}/{stats.MaxCooldownReduction}";
        moveSpeedText.text = $"{stats.currentMoveSpeed}/{stats.MaxMoveSpeed}";
        attackRangeText.text = $"{stats.currentAttackRange}/{stats.MaxAttackRange}";
    }
}
