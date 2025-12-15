using UnityEngine;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
       //ResetAll();

      LoadGame();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetAll();
        }
    }
    public void SaveGame()
    {
        Debug.Log("Saving game ...");
        DialogueBlockNpcHandler.Instance?.SaveDialogueProgress();
        RelationshipManager.Instance?.SaveRelationship();
        MissionManager.Instance?.SaveMissionsToFile();
        StoryExecutor.Instance?.SaveStoryState();
        GameFlowManager.Instance?.SaveStoryBlockID();
    }

    public void LoadGame()
    {
        // nếu các Manager tự load ở Awake thì không cần gọi
        DialogueBlockNpcHandler.Instance?.LoadDialogueProgress();
        RelationshipManager.Instance?.LoadRelationship();
        MissionManager.Instance?.LoadMissionsFromFile();
        StoryExecutor.Instance?.RestoreStoryState();
        GameFlowManager.Instance?.LoadStoryBlockID();

    }

    public void ResetAll()
    {
        RelationshipManager.Instance?.ResetRelationship();
        DialogueBlockNpcHandler.Instance?.ResetDialogueProgress();
        MissionManager.Instance?.ClearAllMissions();
        StoryExecutor.Instance?.ClearStoryState();
        GameFlowManager.Instance?.ResetStoryBlockID();
        SaveGame();
    }
}
