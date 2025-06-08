using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Animator pauseMenu;
    [SerializeField] Animator cursor;
    [SerializeField] Animator leftHand;
    [SerializeField] Transform leftCursor;
    bool is_animating = false;
    private float animationTimer = 1;
    void Update()
    {
        AnimationTimer();
        if (Input.GetKeyDown(KeyCode.Escape) && !is_animating)
        {
            if (!GameManager.pausedGame)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.GetComponent<Image>().enabled = true;
        is_animating = true;
        animationTimer = 0;
        GameManager.pausedGame = true;
        pauseMenu.SetTrigger("Pause");
        cursor.SetTrigger("Pause");
        //leftCursor.SetTrigger("Pause");
        leftHand.SetTrigger("Pause");
    }

    public void Resume()
    {
        is_animating = true;
        animationTimer = 0;
        GameManager.pausedGame = false;
        pauseMenu.SetTrigger("Unpause");
        cursor.SetTrigger("Unpause");
        leftHand.SetTrigger("Unpause");
    }

    void AnimationTimer()
    {
        if (is_animating)
        {
            animationTimer += Time.deltaTime;
            if (!GameManager.pausedGame)
            {
                leftCursor.position = Vector3.Lerp(leftCursor.position, new Vector3(-500, 0, 0), animationTimer);
            }
            if (animationTimer > 1)
            {
                is_animating = false;
            }
        }
    }
}
