using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Story/Composite Story Block")]
public class CompositeStoryBlock : ScriptableObject
{
    public string blockID;
    public List<StoryActionBlock> actions = new List<StoryActionBlock>();
}
