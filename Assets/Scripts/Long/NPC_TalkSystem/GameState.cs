using UnityEngine;

public enum StoryPhase { None, Start, MeetKing, SaveVillage, UnlockMystery, FinalBattle }

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    public StoryPhase CurrentPhase = StoryPhase.Start;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
