using System;

/// <summary>
/// The session-level state of a single playthrough of a level: paused / running,
/// game-over, level-cleared. Mutating methods raise the matching event so
/// observers can react without polling every frame.
/// </summary>
public interface IGameState
{
    bool IsPaused { get; }
    bool IsGameOver { get; }
    bool IsLevelCleared { get; }

    event Action Paused;
    event Action Resumed;
    event Action PlayerDied;
    event Action LevelCleared;

    void SetPaused(bool paused);
    void TriggerGameOver();
    void TriggerLevelCleared();
}
