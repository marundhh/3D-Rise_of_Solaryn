using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtonSoundAuto : MonoBehaviour
{
    [Header("Danh sách UI Root (Parent chứa các nút)")]
    [SerializeField] private Transform[] uiRoots;
    // Kéo tất cả UI cha vào đây

    private void Awake()
    {
        // Gắn sự kiện khi scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Bỏ đăng ký khi object bị destroy
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddSoundToAllButtons();
    }

    private void AddSoundToAllButtons()
    {
        if (uiRoots == null || uiRoots.Length == 0)
        {
            Debug.LogWarning("UIButtonSoundAuto: Chưa gán UI Root nào trong Inspector");
            return;
        }

        int totalButtons = 0;

        foreach (Transform root in uiRoots)
        {
            if (root == null) continue;

            Button[] buttons = root.GetComponentsInChildren<Button>(true); // true = lấy cả inactive
            totalButtons += buttons.Length;

            foreach (Button btn in buttons)
            {
                btn.onClick.AddListener(() =>
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.PlayClipSFX();
                });
            }
        }

        Debug.Log($"Đã gắn âm thanh cho {totalButtons} nút trong {uiRoots.Length} UI root.");
    }
}
