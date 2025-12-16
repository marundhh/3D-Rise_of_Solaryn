using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class PlayerDeathUI : MonoBehaviour
{
    public static PlayerDeathUI Instance;
    [Header("UI Elements")]
    public CanvasGroup panelBackground; // CanvasGroup cho panel nền
    public TextMeshProUGUI deathText;   // Hoặc Text nếu không dùng TMP

    [Header("Animation Settings")]
    public float fadeDuration = 1f;       // Thời gian fade panel
    public float textScaleDuration = 2f;  // Thời gian scale text (chậm hơn)
    public float delayBeforeReset = 1f;   // Thời gian chờ trước khi reset

    private bool isTriggered = false;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ instance khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đi
        }
    }
    private void Start()
    {
        panelBackground.gameObject.SetActive(false); // Ẩn panel nền ban đầu
    }

    public void ShowDeathScreen()
    {
        if (isTriggered) return;
        isTriggered = true;
        panelBackground.gameObject.SetActive(true); // Hiển thị panel nền
        // Reset trạng thái ban đầu
        panelBackground.alpha = 0;
        deathText.alpha = 0;
        deathText.rectTransform.localScale = Vector3.one * 0.3f; // Bắt đầu rất nhỏ

        // Fade panel nền
        panelBackground.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);

        // Tạo Sequence để điều khiển hiệu ứng chữ
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(fadeDuration); // Chờ panel fade xong

        // Fade chữ + phóng to chậm
        seq.Append(
            deathText.DOFade(1f, 3f) // Fade mượt
        );
        seq.Join(
            deathText.rectTransform
            .DOScale(1f, textScaleDuration) // Phóng to từ nhỏ -> to
            .SetEase(Ease.OutCubic)         // Chậm dần ở cuối
        );

        // Chờ trước khi reset scene
        seq.AppendInterval(delayBeforeReset);
        seq.AppendCallback(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            GameStateManager.Instance.LoadGame();
        });
        seq.AppendInterval(delayBeforeReset);
        seq.AppendCallback(() =>
        {
            panelBackground.gameObject.SetActive(false);
        });
    }
}
