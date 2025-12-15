
using DG.Tweening;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class NPCInteract : MonoBehaviour
{


    public NPCDialogueSet dialogueSet;
    public bool canTalk;
    public bool isTalking;
    private void Update()
    {
        if (canTalk  && Input.GetKeyDown(KeyCode.F) )
        {
            Debug.Log(DialogueBlockNpcHandler.Instance.GetDialogueID(dialogueSet.npcName));
            // Find the first available block
            SetDialogue();
        }
    }
    public void SetDialogue()
    {
        foreach (var block in dialogueSet.allDialogues)
        {
            if (block.dialogueID == DialogueBlockNpcHandler.Instance.GetDialogueID(dialogueSet.npcName))
            {
                isTalking = true;
                Debug.Log("Dialogue start" + DialogueBlockNpcHandler.Instance.GetDialogueID(dialogueSet.npcName));
                DialogueManager.Instance.StartDialogue(dialogueSet.npcName, block);
                Interact();
                break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isTalking)
        {
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0; 
            transform.rotation = Quaternion.LookRotation(direction);
            canTalk = true;
            if (string.IsNullOrEmpty(dialogueSet.npcName)) return;
            DialogueManager.Instance.OpenCanInteract(dialogueSet.npcName);
        }
    }

    public void Interact()
    {
        GameEventSystem.Dispatch(new TalkToNPCEvent(dialogueSet.npcName));
    }

    private void OnTriggerExit(Collider other)
    {
        canTalk = false;
        isTalking = false;
        DialogueManager.Instance.CloseCanInteract();
        DialogueManager.Instance.CloseDialogueWithReset();
    }
}
