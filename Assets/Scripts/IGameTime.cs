using System;

/// <summary>
/// Tracks the player's countdown (which is also their health).
/// Implementations expose the current value, allow safe addition/subtraction,
/// and raise an event whenever the value changes.
/// </summary>
public interface IGameTime
{
    float GetTime();
    float MaxTime { get; }

    event Action<float> TimeChanged;

    void AddTime(float time);
    bool ReduceTime(float time);
}
