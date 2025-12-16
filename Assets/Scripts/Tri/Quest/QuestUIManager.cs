using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestUIManager : MonoBehaviour
{
    public GameObject avatarUI;

    private bool isCharacterOpen = false;

    private void Awake()
    {
        if (avatarUI == null)
        {
            avatarUI = GameObject.Find("AvatarUI");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCharacterPanel();
        }
    }

    public void ToggleCharacterPanel()
    {
        if (avatarUI == null) return;

        isCharacterOpen = !isCharacterOpen;
        avatarUI.SetActive(isCharacterOpen);
    }

    public void CloseCharacterPanel()
    {
        if (avatarUI == null) return;

        isCharacterOpen = false;
        avatarUI.SetActive(false);
    }
}
