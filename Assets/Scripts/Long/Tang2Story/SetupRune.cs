using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupRune : MonoBehaviour
{
    public string runeName;
    public string storyBlockID;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            DialogueManager.Instance.OpenCanInteract(runeName);
        }
        
    } 
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameEventSystem.Dispatch(new TalkToNPCEvent(runeName));
                GameFlowManager.Instance.CallSetupStory(storyBlockID);
                DialogueManager.Instance.CloseCanInteract();
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
