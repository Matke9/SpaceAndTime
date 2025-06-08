using UnityEngine;
using UnityEngine.Events;

public class GameTimeManager : MonoBehaviour
{
    private static float gameTime = 75f; 
    
    void Update()
    {
        if(GameManager.pausedGame == false)
           gameTime -= Time.deltaTime;
    }

    public static float GetTime()
    {
        return gameTime;
    }
    
    public static void AddTime(float time)
    {
        if (GameManager.pausedGame == false)
        {
            gameTime += time;
            if (gameTime >= 75)
            {
                gameTime = 75;
            }
        }
    }

    public static bool ReduceTime(float time)
    {
        if (gameTime - time > 0)
        {
            gameTime -= time;
            return true;
        }
        else
        {
            GameManager.GameOver();
            Debug.Log("Died"); 
            gameTime = 0;
            return false;
        }
    }
}
