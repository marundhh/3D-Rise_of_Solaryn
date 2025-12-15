using UnityEngine;

public class ClassManager : MonoBehaviour
{
    [Header("Character Class Data")]
    public CharacterClassData selectedClassData;

    public Transform model;

    private void Awake()
    {
        InitData();
        InitModel();
        selectedClassData.Init();   
    }

    private void InitData()
    {
        selectedClassData = ClassDataPlayerChoose.instance.characterClassData;
    }

    private void InitModel()
    {
        if (selectedClassData == null || selectedClassData.model == null)
        {
            Debug.LogWarning("CharacterClassData or modelClass is null");
            return;
        }

        GameObject modelInstance = Instantiate(selectedClassData.model, model);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.identity;
    }
}
