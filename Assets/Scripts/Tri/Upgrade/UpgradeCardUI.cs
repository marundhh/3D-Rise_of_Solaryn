using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("Refs")]
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Button selectButton;

    [Header("Rarity Colors (tùy chỉnh được)")]
    public Color commonColor = Color.green;
    public Color rareColor = Color.yellow;
    public Color epicColor = Color.red;
    public Color defaultColor = Color.gray;

    private UpgradeOption upgradeData;

 
    public void Init(UpgradeOption option, System.Action<UpgradeOption> onSelect)
    {
        upgradeData = option;

        if (icon != null) icon.sprite = option.icon;
        if (nameText != null) nameText.text = option.name;
        if (descText != null) descText.text = option.description;

        ApplyRarityColor(option.rarity);

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect?.Invoke(upgradeData));
    }

  
    public void ApplyRarityColor(UpgradeRarity rarity)
    {
        if (background == null) return;

        switch (rarity)
        {
            case UpgradeRarity.Common:
                background.color = commonColor;
                break;
            case UpgradeRarity.Rare:
                background.color = rareColor;
                break;
            case UpgradeRarity.Epic:
                background.color = epicColor;
                break;
            default:
                background.color = defaultColor;
                break;
        }
    }

#if UNITY_EDITOR
  
    private void OnValidate()
    {
       
        if (upgradeData != null)
        {
            ApplyRarityColor(upgradeData.rarity);
        }
        else
        {
         
            if (background != null)
                background.color = commonColor;
        }

        if (nameText != null && upgradeData != null)
            nameText.text = upgradeData.name;
        if (descText != null && upgradeData != null)
            descText.text = upgradeData.description;
        if (icon != null && upgradeData != null)
            icon.sprite = upgradeData.icon;
    }
#endif
}