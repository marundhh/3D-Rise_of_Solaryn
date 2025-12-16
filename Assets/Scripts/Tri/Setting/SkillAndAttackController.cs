using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillAndAttackController : MonoBehaviour
{
    public float skillCooldownTime = 5f;
    private float skillCooldownTimer;
    private bool isSkillCooldown = false;

    public Image skillCooldownImage;
    public TextMeshProUGUI skillCooldownText;

    void Start()
    {
        // Ẩn UI cooldown khi vào game
        if (skillCooldownImage != null)
            skillCooldownImage.fillAmount = 0;

        if (skillCooldownText != null)
            skillCooldownText.text = "";
    }

    void Update()
    {
        HandleNormalAttack();
        HandleSkill();
    }

    void HandleNormalAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Đòn đánh thường!");
        }
    }

    void HandleSkill()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSkillCooldown)
        {
            UseSkill();
        }

        if (isSkillCooldown)
        {
            skillCooldownTimer -= Time.deltaTime;

            if (skillCooldownImage != null)
                skillCooldownImage.fillAmount = skillCooldownTimer / skillCooldownTime;

            if (skillCooldownText != null)
                skillCooldownText.text = skillCooldownTimer.ToString("F2"); // Hiển thị 2 chữ số sau dấu chấm


            if (skillCooldownTimer <= 0f)
            {
                isSkillCooldown = false;

                if (skillCooldownImage != null)
                    skillCooldownImage.fillAmount = 0;

                if (skillCooldownText != null)
                    skillCooldownText.text = "";
            }
        }
    }

    void UseSkill()
    {
        Debug.Log("Kỹ năng được kích hoạt!");
        isSkillCooldown = true;
        skillCooldownTimer = skillCooldownTime;

        if (skillCooldownImage != null)
            skillCooldownImage.fillAmount = 1f;
    }
}
