using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;
    public string currentStoryBlockID;
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private async void Start()
    {
        await SceneTransitionManager.Instance.DoFadeTransition(async () =>
        {
            await StoryManager.Instance.PlayBlock(currentStoryBlockID);
        });
    }

    // none SceneTransition
    public async Task SetupStory(string currentStoryBlockID)
    {
        this.currentStoryBlockID = currentStoryBlockID;
        await StoryManager.Instance.PlayBlock(currentStoryBlockID);
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            await CallSetupStory(currentStoryBlockID);
        }
    }

    //SceneTransition
    public async Task CallSetupStory(string StoryBlockID)
    {
        await SceneTransitionManager.Instance.DoFadeTransition(async () =>
        {
            currentStoryBlockID = StoryBlockID;
            await StoryManager.Instance.PlayBlock(StoryBlockID);
            GameStateManager.Instance.SaveGame();
        });
    }
    public async Task CallSetupStoryWithOutSave(string StoryBlockID)
    {
        await SceneTransitionManager.Instance.DoFadeTransition(async () =>
        {
            currentStoryBlockID = StoryBlockID;
            await StoryManager.Instance.PlayBlock(StoryBlockID);
        });
    }

    public async void LoadStoryBlockID()
    {
        if (PlayerPrefs.HasKey("CurrentStoryBlockID"))
        {
            currentStoryBlockID = PlayerPrefs.GetString("CurrentStoryBlockID");
            Debug.Log("Loading current story block ID: " + currentStoryBlockID);
            await CallSetupStory(currentStoryBlockID);
        } else
        {
             Debug.Log("Play default ID");
             await CallSetupStory(currentStoryBlockID);
        }
    }
    public void ResetStoryBlockID()
    {
        Debug.Log("Resetting current story block ID");
        currentStoryBlockID = "Story1D101"; // Set to a default or initial block ID
        PlayerPrefs.DeleteKey("CurrentStoryBlockID");
        PlayerPrefs.Save();
    }
    public void SaveStoryBlockID()
    {
        string json = JsonUtility.ToJson(currentStoryBlockID);

        Debug.Log("Saving current story block ID: " + currentStoryBlockID);
        PlayerPrefs.SetString("CurrentStoryBlockID", currentStoryBlockID);
        PlayerPrefs.Save();
    }
}
