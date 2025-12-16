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



    // none SceneTransition
    public async Task CallSetupStoryNoneTransiton(string StoryBlockID)
    {
        if (!string.IsNullOrEmpty(StoryBlockID))
            currentStoryBlockID = StoryBlockID;
        await StoryManager.Instance.PlayBlock(currentStoryBlockID);
        GameStateManager.Instance.SaveGame();
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            await CallSetupStory(currentStoryBlockID);
        }
    }

    public async void NewGame()
    {
        await CallSetupStory(currentStoryBlockID);
    }
    //SceneTransition
    public async Task CallSetupStory(string StoryBlockID)
    {
        await SceneTransitionManager.Instance.DoFadeTransition(async () =>
        {
            if (!string.IsNullOrEmpty(StoryBlockID))
                currentStoryBlockID = StoryBlockID;
            await StoryManager.Instance.PlayBlock(currentStoryBlockID);
            GameStateManager.Instance.SaveGame();
        });
    }
    public async Task CallSetupStoryWithOutSave(string StoryBlockID)
    {
        await SceneTransitionManager.Instance.DoFadeTransition(async () =>
        {
            if (!string.IsNullOrEmpty(StoryBlockID))
                currentStoryBlockID = StoryBlockID;
            await StoryManager.Instance.PlayBlock(currentStoryBlockID);
        });
    }

    public async void LoadStoryBlockID()
    {
        string loadStoryID = await NpcData.instance.LoadCurrentStoryBlockAsync();

        Debug.Log($"LoadStoryBlockID: {loadStoryID}");

        if (!string.IsNullOrEmpty(loadStoryID))
        {
            Debug.Log("PlayerPrefs has CurrentStoryBlockID");
            await CallSetupStory(loadStoryID);
        }
        else
        {
            Debug.Log("PlayerPrefs does not have CurrentStoryBlockID, starting new game");
            await CallSetupStory(currentStoryBlockID);
        }
    }
    public async Task ResetStoryBlockID()
    {
        currentStoryBlockID = string.Empty; // Set to a default or initial block ID
        await NpcData.instance.SaveCurrentStoryBlockAsync(currentStoryBlockID);
    }
    public async Task SaveStoryBlockID()
    {
       await NpcData.instance.SaveCurrentStoryBlockAsync(currentStoryBlockID);
    }
}
