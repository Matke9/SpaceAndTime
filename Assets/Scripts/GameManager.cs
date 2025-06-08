using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool pausedGame = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("paused");
            pausedGame = !pausedGame;
        }
    }

    public static void GameOver()
    {
        
    }
}
