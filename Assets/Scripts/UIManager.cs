using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Animator pauseMenu;
    [SerializeField] Animator cursor;
    bool animating = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !animating)
        {
            GameManager.pausedGame = !GameManager.pausedGame;
            animating = true;
            if (GameManager.pausedGame)
            {
                pauseMenu.SetTrigger("Pause");
                cursor.SetTrigger("Pause");
            }
            else
            {
                pauseMenu.SetTrigger("Unpause");
                cursor.SetTrigger("Unpause");
            }
            StartCoroutine(StopAnimating());
        }
    }

    IEnumerator StopAnimating()
    {
        yield return new WaitForSeconds(1);
        animating = false;
    }
}
