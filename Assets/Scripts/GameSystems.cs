using UnityEngine;

/// <summary>
/// Per-scene service locator for the small set of session-scoped services
/// (time, state). Concrete implementations register themselves in Awake
/// and unregister in OnDestroy. If nothing has registered yet, a lazy
/// scene lookup is performed as a fallback.
/// </summary>
public static class GameSystems
{
    private static IGameTime _time;
    private static IGameState _state;

    public static IGameTime Time
    {
        get
        {
            if (_time == null)
                _time = Object.FindFirstObjectByType<GameTimeManager>();
            return _time;
        }
    }

    public static IGameState State
    {
        get
        {
            if (_state == null)
                _state = Object.FindFirstObjectByType<GameManager>();
            return _state;
        }
    }

    public static void RegisterTime(IGameTime time) => _time = time;
    public static void RegisterState(IGameState state) => _state = state;

    public static void UnregisterTime(IGameTime time)
    {
        if (_time == time) _time = null;
    }

    public static void UnregisterState(IGameState state)
    {
        if (_state == state) _state = null;
    }
}
