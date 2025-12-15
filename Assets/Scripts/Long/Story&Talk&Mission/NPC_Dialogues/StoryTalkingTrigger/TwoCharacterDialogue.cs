using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoCharacterDialogue: MonoBehaviour
{
    public NPCDialogueSet dialogueSet;
    public void StartDialogue()
    {
        if (dialogueSet == null || dialogueSet.allDialogues.Count == 0)
            return;
        DialogueManager.Instance.StartDialogue(dialogueSet.npcName, dialogueSet.allDialogues[0]);
    }
}
