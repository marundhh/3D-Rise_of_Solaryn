using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinionNameManager : MonoBehaviour
{
    public MinionData minionData;            // ScriptableObject chứa tên
    public TextMeshProUGUI nameTextUI;     // Text để hiện tên (gán trực tiếp)

    void Start()
    {
        if (minionData == null || nameTextUI == null)
        {
            Debug.LogWarning("Thiếu MinionData hoặc Text UI!");
            return;
        }

        nameTextUI.text = minionData.displayName;
    }
}
