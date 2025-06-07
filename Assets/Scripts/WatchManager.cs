using System;
using UnityEngine;
using UnityEngine.UI;

public class WatchManager : MonoBehaviour
{
    private Image circle;

    private void Start()
    {
        circle = GetComponent<Image>();
    }

    void Update()
    {
        circle.fillAmount = .75f - (GameTimeManager.gameTime / 100);
    }
}
