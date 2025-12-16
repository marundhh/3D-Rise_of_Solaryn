using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        Debug.Log("Starting game ..." + PlayerData.instance.isNewGame);
        if (!PlayerData.instance.isNewGame)
        {
            LoadGame();
            Debug.Log("Loading game ...");
        }
        else
        {
            NewGame();
            PlayerData.instance.isNewGame = false;
            Debug.Log("New game ...");
        }
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

    public void NewGame()
    {
       GameFlowManager.Instance?.NewGame();
    }

    public void LoadGame()
    {
        DialogueBlockNpcHandler.Instance?.LoadDialogueProgress();
        RelationshipManager.Instance?.LoadRelationship();
        MissionManager.Instance?.LoadMissionsFromFile();
        StoryExecutor.Instance?.LoadStoryState();
        GameFlowManager.Instance?.LoadStoryBlockID();
    }

    public void ResetAll()
    {
        RelationshipManager.Instance?.ResetRelationship();
        DialogueBlockNpcHandler.Instance?.ResetDialogueProgress();
        MissionManager.Instance?.ClearAllMissions();
        StoryExecutor.Instance?.ClearStoryState();
        GameFlowManager.Instance?.ResetStoryBlockID();
    }
}
