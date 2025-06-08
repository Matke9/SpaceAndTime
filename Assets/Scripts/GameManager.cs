using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool pausedGame = false;
    public static bool gameOver = false;
    public static bool passed = false;

    private void Start()
    {
        gameOver = false;
        passed = false;
        pausedGame = false;
    }

    private void Update()
    {
        if (gameOver == true && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (passed == true && Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneManager.GetActiveScene().buildIndex + 1 < 2)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
    }

    public static void GameOver()
    {
        pausedGame = true;
        gameOver = true;
    }
    public static void NextLevel()
    {
        pausedGame = true;
        passed = true;
    }
}
