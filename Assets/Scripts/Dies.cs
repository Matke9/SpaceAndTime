using UnityEngine;

public class Dies : MonoBehaviour
{
    private GameObject dies;
    private IGameState _state;

    private void Start()
    {
        dies = transform.Find("Dies").gameObject;
        dies.SetActive(false);

        _state = GameSystems.State;
        if (_state != null)
            _state.PlayerDied += OnPlayerDied;
    }

    private void OnDestroy()
    {
        if (_state != null)
            _state.PlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        dies.SetActive(true);
    }
}
