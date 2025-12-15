using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public Button selectButton;

    private UpgradeOption upgradeData;

    public void Init(UpgradeOption option, System.Action<UpgradeOption> onSelect)
    {
        upgradeData = option;
        icon.sprite = option.icon;
        nameText.text = option.name;
        descText.text = option.description;

        // Set màu theo độ hiếm
        switch (option.rarity)
        {
            case UpgradeRarity.Common: background.color = Color.green; break;
            case UpgradeRarity.Rare: background.color = Color.yellow; break;
            case UpgradeRarity.Epic: background.color = Color.red; break;
        }

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => onSelect(upgradeData));
    }
}
