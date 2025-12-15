using UnityEngine;

public class TestLevelUp : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Gaining EXP...");
            FindObjectOfType<PlayerLevel>().GainExp(999);
        }
    }
}
