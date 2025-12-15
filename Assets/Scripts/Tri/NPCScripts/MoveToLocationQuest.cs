using UnityEngine;

[CreateAssetMenu(fileName = "MoveQuest", menuName = "Game/Move To Location Quest")]
public class MoveToLocationQuest : ScriptableObject
{
    
    public Vector3 targetLocation;
    
    public string questName;
    [TextArea] public string description;
    public bool isComplete = false;

    public void Complete()
    {
        isComplete = true;
        Debug.Log("✅ Nhiệm vụ hoàn thành!");
    }
}
