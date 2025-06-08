using System;
using UnityEngine;

public class Dies : MonoBehaviour
{
    private GameObject dies;

    private void Start()
    {
        dies = transform.Find("Dies").gameObject;
        dies.SetActive(false);
    }

    void Update()
    {
        if (GameManager.gameOver)
        {
            dies.SetActive(true);
        }
    }
}
