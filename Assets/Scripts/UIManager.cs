using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Animator pauseMenu;
    [SerializeField] Animator cursor;
    bool paused = false;
    bool animating = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !animating)
        {
            paused = !paused;
            animating = true;
            if (paused)
            {
                pauseMenu.SetTrigger("Pause");
                cursor.SetTrigger("Pause");
            }
            else
            {
                pauseMenu.SetTrigger("Unpause");
                cursor.SetTrigger("Unpause");
            }
            //StartCoroutine(StopAnimating());
        }
    }

    IEnumerator StopAnimating()
    {
        yield return new WaitForSeconds(1);
        animating = false;
    }
}
