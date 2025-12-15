using System;

public static class GameEventSystem
{
    public static Action<GameEvent> OnGameEvent;

    public static void Dispatch(GameEvent e)
    {
        OnGameEvent?.Invoke(e);
    }
}
