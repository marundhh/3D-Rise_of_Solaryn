using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject characterPanel;
    public GameObject settingsButton;   // Nút mở bảng cài đặt
    public GameObject settingsPanel;    // Panel chứa UI cài đặt
    public GameObject closeAvatarButton; // Nút đóng avatar (nếu bạn muốn ẩn riêng)

    private bool isQuestOpen = false;
    private bool isCharacterOpen = false;
    private bool isSettingsOpen = false;

    void Update()
    {
        // Mở/đóng bảng nhiệm vụ bằng phím J
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleQuestPanel();
        }

        // Mở/đóng bảng nhân vật bằng phím I
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCharacterPanel();
        }
    }

    public void ToggleQuestPanel()
    {
        isQuestOpen = !isQuestOpen;
        questPanel.SetActive(isQuestOpen);
    }

    public void CloseQuestPanel()
    {
        isQuestOpen = false;
        questPanel.SetActive(false);
    }

    public void ToggleCharacterPanel()
    {
        isCharacterOpen = !isCharacterOpen;
        characterPanel.SetActive(isCharacterOpen);

        if (settingsButton != null)
            settingsButton.SetActive(isCharacterOpen);

        if (closeAvatarButton != null)
            closeAvatarButton.SetActive(isCharacterOpen);
    }

    public void CloseCharacterPanel()
    {
        isCharacterOpen = false;
        characterPanel.SetActive(false);

        if (settingsButton != null)
            settingsButton.SetActive(false);

        if (closeAvatarButton != null)
            closeAvatarButton.SetActive(false);
    }

    public void ToggleSettingsPanel()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);
    }

    public void CloseSettingsPanel()
    {
        isSettingsOpen = false;
        settingsPanel.SetActive(false);
    }
}
