using UnityEngine;
using System.Collections;


[CreateAssetMenu(fileName = "NewCharacterClassData", menuName = "Data/CharacterClassData"), System.Serializable]
public class CharacterClassData : ScriptableObject
{
    [Header("Class Info")]
    public float attackRange;
    public GameObject model;
    public ClassType className;

    [Header("Class References")]
    public Avatar avatar;

    [Header("Audio")]
    public AudioClip attackSound;

    public void Init(MonoBehaviour runner)
    {
        runner.StartCoroutine(SetAvatarWhenReady());
    }

    private IEnumerator SetAvatarWhenReady()
    {
        while (AnimationManager.instance?.animator == null)
        {
            yield return null;
        }
        AnimationManager.instance.animator.avatar = avatar;
    }
}
