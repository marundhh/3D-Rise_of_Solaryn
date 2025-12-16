using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Settings")]
    public string scenePlay;
    public string sceneNew;

    [Header("UI")]
    public GameObject popupNewGame;

    // ▶ PLAY GAME
    public void OnClickPlay()
    {
        // Nếu muốn kiểm tra có save hay chưa thì giữ
        if (PlayerData.instance != null && PlayerData.instance.maxHealth <= 0)
        {
            popupNewGame?.SetActive(true);
            return;
        }

        LoadScene(scenePlay);
    }

    // ▶ SELECT (nếu bạn cần)
    public void OnClickSelect()
    {
        LoadScene(scenePlay);
    }

    // ▶ NEW GAME
    public void OnClickNew()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.isNewGame = true;
        }

        LoadScene(sceneNew);
    }

    // ▶ EXIT GAME
    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Thoát Game");
    }

    // 🔁 Hàm load scene dùng chung
    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene chưa được gán!");
            return;
        }

        LoadingScene.Instance.StartLoading(sceneName);
    }
}
