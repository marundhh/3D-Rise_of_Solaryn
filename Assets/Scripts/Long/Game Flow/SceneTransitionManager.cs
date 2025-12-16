using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public CanvasGroup fadeCanvas;

    public TextMeshProUGUI noityText;

    public static SceneTransitionManager Instance;
    private void Awake()
    {
        if (Instance != null ) { Destroy(gameObject); return; }
        Instance = this;
    }
    public async UniTask DoFadeTransition(Func<UniTask> onBlockAction)
    {
        await FadeOut();
        if (onBlockAction != null)
            await onBlockAction();
        await FadeStay();
        await FadeIn();
    }

    public async UniTask FadeOut()
    {
        PlayerMovement.isInputLocked = true;
        fadeCanvas.gameObject?.SetActive(true);
        await fadeCanvas.DOFade(1f, 1f).AwaitForCompletion();
    }


    public async UniTask FadeStay()
    {
        if (fadeCanvas)
        {
            fadeCanvas.gameObject.SetActive(true);
            await fadeCanvas.DOFade(1f, 0.5f).AwaitForCompletion();
        }
    }

    public async UniTask FadeIn()
    {
        PlayerMovement.isInputLocked = false;
        noityText.text = ""; // Xóa thông báo 
        await fadeCanvas.DOFade(0f, 1f).AwaitForCompletion();
        DOTween.Kill(fadeCanvas); // Dọn tween tránh lỗi
        if(fadeCanvas != null)
            fadeCanvas.gameObject.SetActive(false);
    }

    public void SetNotification(string noity)
    {
        noityText.text = noity;
    }

    public void LoadScene(string sceneName)
    {
       SceneManager.LoadScene(sceneName);
    }
}
