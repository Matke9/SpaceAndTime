using UnityEngine;
using UnityEngine.UI;

public class WatchManager : MonoBehaviour
{
    private Image circle;
    private IGameTime _time;

    private void Start()
    {
        circle = GetComponent<Image>();
        _time = GameSystems.Time;
        if (_time != null)
        {
            _time.TimeChanged += OnTimeChanged;
            OnTimeChanged(_time.GetTime());
        }
    }

    private void OnDestroy()
    {
        if (_time != null)
            _time.TimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(float remaining)
    {
        circle.fillAmount = .75f - (remaining / 100f);
    }
}
