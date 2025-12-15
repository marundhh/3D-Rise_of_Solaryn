using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;

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
    public async UniTask DoFadeTransition(Func<UniTask> onBlackAction)
    {
        await FadeOut();
        if (onBlackAction != null)
            await onBlackAction();
        await FadeStay();
        await FadeIn();
    }

    private async UniTask FadeOut()
    {
        fadeCanvas.gameObject.SetActive(true);
        await fadeCanvas.DOFade(1f, 1f).AwaitForCompletion();
    }


    private async UniTask FadeStay()
    {
        fadeCanvas.gameObject.SetActive(true); 
        await fadeCanvas.DOFade(1f, 0.5f).AwaitForCompletion();
    }

    private async UniTask FadeIn()
    {
        noityText.text = ""; // Xóa thông báo 
        await fadeCanvas.DOFade(0f, 1f).AwaitForCompletion();
        DOTween.Kill(fadeCanvas); // Dọn tween tránh lỗi
        fadeCanvas.gameObject.SetActive(false);
    }

    public void SetNotification(string noity)
    {
        noityText.text = noity;
    }
}
