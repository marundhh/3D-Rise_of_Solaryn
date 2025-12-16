using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStory : MonoBehaviour
{
    public string startStoryBlockID;
    public bool isTriggerOnce = true;
    private async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameFlowManager.Instance != null && isTriggerOnce)
        {
            isTriggerOnce = false;
            if (!string.IsNullOrEmpty(startStoryBlockID))
            {
                await GameFlowManager.Instance.CallSetupStoryWithOutSave(startStoryBlockID);
                // Check if the component TwoCharacterDialogue exists
                try
                {
                    if (TryGetComponent<TwoCharacterDialogue>(out var twoDialogue) && twoDialogue)
                    {
                        twoDialogue.StartDialogue();
                    }
                } catch (System.Exception ex)
                {
                    Debug.Log($"Error starting dialogue: {ex.Message}");
                }

                return;
            }
            else
            {
                GetComponent<TwoCharacterDialogue>().StartDialogue();
            }
        }

           
    }
}
