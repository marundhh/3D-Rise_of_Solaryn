using UnityEngine;
using UnityEngine.UI;

public class QuestDestination : MonoBehaviour
{
    public MoveToLocationQuest quest;
    public GameObject completeTextUI; // Text hiện "Hoàn thành nhiệm vụ"

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !quest.isComplete)
        {
            quest.Complete();

            if (completeTextUI != null)
            {
                completeTextUI.SetActive(true);
                Invoke(nameof(HideUI), 3f);
            }
        }
    }

    void HideUI()
    {
        if (completeTextUI != null)
            completeTextUI.SetActive(false);
    }
}
