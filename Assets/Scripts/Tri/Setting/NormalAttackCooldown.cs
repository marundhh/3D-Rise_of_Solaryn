using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NormalAttackCooldown : MonoBehaviour
{
    public float attackCooldownTime = 1.5f; // Thời gian hồi chiêu đánh thường
    private float attackCooldownTimer;
    private bool isAttackCooldown = false;

    public Image attackCooldownImage;           // Vòng hồi chiêu nếu có
    public TextMeshProUGUI attackCooldownText;  // Text thời gian còn lại
    public Animator animator;                   // Animator nếu có

    void Update()
    {
        HandleNormalAttack();
        UpdateCooldownUI();
    }

    void HandleNormalAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttackCooldown)
        {
            // Thực hiện đánh thường
           

            if (animator != null)
                animator.SetTrigger("Attack");

            // Bắt đầu hồi chiêu
            isAttackCooldown = true;
            attackCooldownTimer = attackCooldownTime;

            if (attackCooldownImage != null)
                attackCooldownImage.fillAmount = 1f;
        }

        // Đếm ngược cooldown
        if (isAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;

            if (attackCooldownTimer <= 0f)
            {
                isAttackCooldown = false;

                if (attackCooldownImage != null)
                    attackCooldownImage.fillAmount = 0;

                if (attackCooldownText != null)
                    attackCooldownText.text = "";
            }
        }
    }

    void UpdateCooldownUI()
    {
        if (isAttackCooldown)
        {
            if (attackCooldownImage != null)
                attackCooldownImage.fillAmount = attackCooldownTimer / attackCooldownTime;

            if (attackCooldownText != null)
                attackCooldownText.text = attackCooldownTimer.ToString("F2"); // Hiển thị số giây với 2 chữ số thập phân
        }
    }

}
