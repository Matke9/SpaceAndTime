using UnityEngine;
using UnityEngine.Events;

public class GameTimeManager : MonoBehaviour
{
    public static float gameTime = 75f; 
    
    void Update()
    {
           gameTime -= Time.deltaTime;
    }
}
