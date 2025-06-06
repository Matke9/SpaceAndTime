using UnityEngine;
using UnityEngine.Events;

public class GameTimeManager : MonoBehaviour
{
    public static float gameTime = 50f; 
    public static UnityEvent gameTimeUpdate = new UnityEvent();
    
    void Update()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (scroll > 0f)
        {
            if (gameTime < 100f)
            {
                gameTime += 1f;
            }
            Debug.Log(gameTime);
            gameTimeUpdate.Invoke();
        }
        else if (scroll < 0f)
        {
            if (gameTime > 0f)
            {
                gameTime -= 1f;
            }
            Debug.Log(gameTime);
            gameTimeUpdate.Invoke();
        }   
    }
}
