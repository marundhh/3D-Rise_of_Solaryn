using UnityEngine;


[CreateAssetMenu(fileName = "NewCharacterClassData", menuName = "Data/CharacterClassData"), System.Serializable]
public class CharacterClassData : ScriptableObject
{
    [Header("Class Info")]
    public float attackRange;
    public GameObject model;

    [Header("Class References")]
    public Avatar avatar;

    public void Init()
    {
        AnimationManager.instance.animator.avatar = avatar;
    }
}
