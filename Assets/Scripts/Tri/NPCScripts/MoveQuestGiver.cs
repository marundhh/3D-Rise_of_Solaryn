using UnityEngine;

public class MoveQuestGiver : MonoBehaviour
{
    public MoveToLocationQuest quest;
    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!quest.isComplete)
            {
                Debug.Log("📍 Nhiệm vụ: " + quest.description);
            }
            else
            {
                Debug.Log("🎉 Bạn đã hoàn thành nhiệm vụ!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
