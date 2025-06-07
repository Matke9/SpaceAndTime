using UnityEngine;
using UnityEngine.Events;

public class GameTimeManager : MonoBehaviour
{
    private static float gameTime = 75f; 
    
    void Update()
    {
           gameTime -= Time.deltaTime;
    }

    public static float GetTime()
    {
        return gameTime;
    }
    
    public static void AddTime(float time)
    {
        gameTime += time;
        if (gameTime >= 75)
        {
            gameTime = 75;
        }
    }

    public static void ReduceTime(float time)
    {
        gameTime -= time;
        if (gameTime <= 0)
        {
            gameTime = 0;
        }
    }
}
