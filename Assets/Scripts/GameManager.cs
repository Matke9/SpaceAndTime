using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour, IGameState
{
    private bool _isPaused;
    private bool _isGameOver;
    private bool _isLevelCleared;

    public bool IsPaused => _isPaused;
    public bool IsGameOver => _isGameOver;
    public bool IsLevelCleared => _isLevelCleared;

    public event Action Paused;
    public event Action Resumed;
    public event Action PlayerDied;
    public event Action LevelCleared;

    private void Awake()
    {
        _isPaused = false;
        _isGameOver = false;
        _isLevelCleared = false;
        GameSystems.RegisterState(this);
    }

    private void OnDestroy()
    {
        GameSystems.UnregisterState(this);
    }

    private void Update()
    {
        if (_isLevelCleared && Input.GetKeyDown(KeyCode.Space))
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;
            if (next < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(next);
        }
    }

    public void SetPaused(bool paused)
    {
        if (_isPaused == paused) return;
        _isPaused = paused;
        if (paused) Paused?.Invoke();
        else Resumed?.Invoke();
    }

    public void TriggerGameOver()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        bool wasRunning = !_isPaused;
        _isPaused = true;
        PlayerDied?.Invoke();
        if (wasRunning) Paused?.Invoke();
    }

    public void TriggerLevelCleared()
    {
        if (_isLevelCleared) return;
        _isLevelCleared = true;
        bool wasRunning = !_isPaused;
        _isPaused = true;
        LevelCleared?.Invoke();
        if (wasRunning) Paused?.Invoke();
    }
}
