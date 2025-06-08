using System;
using UnityEngine;

public class Wins : MonoBehaviour
{
    private GameObject wins;

    private void Start()
    {
        wins = transform.Find("Wins").gameObject;
        wins.SetActive(false);
    }

    void Update()
    {
        if (GameManager.passed)
        {
            wins.SetActive(true);
        }
    }
}
