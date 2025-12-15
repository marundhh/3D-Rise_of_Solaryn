using UnityEngine;
using TMPro;

public class EnemyNameManager : MonoBehaviour
{
    public EnemyData enemyData;            // ScriptableObject chứa tên
    public TextMeshProUGUI nameTextUI;     // Text để hiện tên (gán trực tiếp)

    void Start()
    {
        if (enemyData == null || nameTextUI == null)
        {
            Debug.LogWarning("Thiếu EnemyData hoặc Text UI!");  
            return;
        }

        nameTextUI.text = enemyData.displayName;
    }
}
