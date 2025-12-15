
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dialogue/DialogueBlock")]
public class DialogueBlock : ScriptableObject
{
    public string dialogueID;
    [TextArea]
    public List<string> lines;
    [TextArea]
    public List<string> lines2;
    public List<DialogueChoice> choices;
    public List<DialogueChoice> choices2;
    public DialogueCondition condition;
}