
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/NPCDialogueSet")]
public class NPCDialogueSet : ScriptableObject
{
    public string npcName;
    public List<DialogueBlock> allDialogues;
}
