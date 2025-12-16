using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMissionChecker : MonoBehaviour
{
    public string m_itemName;

    public string StoryBlockID;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.OpenCanCollect(m_itemName);
           if(Input.GetKeyDown(KeyCode.F))
            {
                GameEventSystem.Dispatch( new ItemCollectedEvent(m_itemName));
                DialogueManager.Instance.CloseCanInteract();
                if (!string.IsNullOrEmpty(StoryBlockID))
                {
                    GameFlowManager.Instance.CallSetupStoryNoneTransiton(StoryBlockID);
                }
                Destroy(gameObject);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.CloseCanInteract();
        }
    }
}
