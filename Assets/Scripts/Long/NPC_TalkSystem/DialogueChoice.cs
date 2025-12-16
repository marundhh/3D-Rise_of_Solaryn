using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public string nextDialogueId = null;
    public string storyBlockID;
    public bool playNextDialogueAutomatically = false;
    public string openMenuID;
    [TextArea]
    public List<string> choiceLines;
    public MissionRaw missionRaw;
    public int relationShipEff;
    public string setFlag;
}