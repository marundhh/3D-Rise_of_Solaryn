using UnityEngine;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    public static UpgradeUI Instance;
    public GameObject panel;
    public List<UpgradeCardUI> cardUIs;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false); // Ẩn bảng khi bắt đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (panel.activeSelf)
                Hide(); // Nếu đang mở thì ẩn
            else
                Show(); // Nếu đang ẩn thì hiện
        }
    }


    public void Show()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        List<UpgradeOption> options = UpgradeManager.Instance.GenerateFixed5Upgrades();

        for (int i = 0; i < cardUIs.Count; i++)
        {
            cardUIs[i].Init(options[i], ApplyUpgrade);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }


    public void ApplyUpgrade(UpgradeOption chosen)
    {
        Debug.Log("Đã chọn nâng cấp: " + chosen.name);
        // Tăng chỉ số nhân vật ở đây...

        Hide();
    }

   
}
