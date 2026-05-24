using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameTimeManager : MonoBehaviour, IGameTime
{
    private float _gameTime;
    private PlayerController _playerController;

    public event Action<float> TimeChanged;

    public float MaxTime => GameSettings.Current.maxTime;

    private void Awake()
    {
        _gameTime = GameSettings.Current.startingTime;
        GameSystems.RegisterTime(this);
    }

    private void Start()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        TimeChanged?.Invoke(_gameTime);
    }

    private void OnDestroy()
    {
        GameSystems.UnregisterTime(this);
    }

    private void Update()
    {
        var state = GameSystems.State;
        if (state == null || state.IsPaused) return;

        if (_gameTime > 0)
        {
            _gameTime -= Time.deltaTime;
            TimeChanged?.Invoke(_gameTime);
        }

        if (_gameTime <= 0)
        {
            _gameTime = 0;
            TimeChanged?.Invoke(_gameTime);
            HandleTimeOut();
        }
    }

    public float GetTime() => _gameTime;

    public void AddTime(float time)
    {
        var state = GameSystems.State;
        if (state != null && state.IsPaused) return;

        _gameTime += time;
        if (_gameTime >= MaxTime)
            _gameTime = MaxTime;
        TimeChanged?.Invoke(_gameTime);
    }

    public bool ReduceTime(float time)
    {
        if (_gameTime - time > 0)
        {
            _gameTime -= time;
            TimeChanged?.Invoke(_gameTime);
            return true;
        }

        _gameTime = 0;
        TimeChanged?.Invoke(_gameTime);
        HandleTimeOut();
        return false;
    }

    private void HandleTimeOut()
    {
        if (_playerController != null) _playerController.Die();
        GameSystems.State?.TriggerGameOver();
    }
}
