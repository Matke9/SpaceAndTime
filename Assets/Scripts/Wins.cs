using UnityEngine;

public class Wins : MonoBehaviour
{
    private GameObject wins;
    private IGameState _state;

    private void Start()
    {
        wins = transform.Find("Wins").gameObject;
        wins.SetActive(false);

        _state = GameSystems.State;
        if (_state != null)
            _state.LevelCleared += OnLevelCleared;
    }

    private void OnDestroy()
    {
        if (_state != null)
            _state.LevelCleared -= OnLevelCleared;
    }

    private void OnLevelCleared()
    {
        wins.SetActive(true);
    }
}
