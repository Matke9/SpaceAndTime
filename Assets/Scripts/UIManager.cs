using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Animator pauseMenu;
    [SerializeField] Animator cursor;
    [SerializeField] Animator leftHand;
    [SerializeField] Transform leftCursor;
    [SerializeField] AudioSource audioSrc;
    bool is_animating = false;
    private float animationTimer = 1;

    void Update()
    {
        AnimationTimer();
        if (Input.GetKeyDown(KeyCode.Escape) && !is_animating)
        {
            bool paused = GameSystems.State?.IsPaused ?? false;
            if (!paused) Pause();
            else Resume();
        }
    }

    public void Pause()
    {
        is_animating = true;
        audioSrc.Play();
        pauseMenu.GetComponent<Image>().enabled = true;
        leftCursor.GetComponent<Image>().enabled = true;
        animationTimer = 0;
        GameSystems.State?.SetPaused(true);
        pauseMenu.SetTrigger("Pause");
        cursor.SetTrigger("Pause");
        leftHand.SetTrigger("Pause");
    }

    public void Resume()
    {
        is_animating = true;
        audioSrc.Play();
        animationTimer = 0;
        GameSystems.State?.SetPaused(false);
        pauseMenu.SetTrigger("Unpause");
        cursor.SetTrigger("Unpause");
        leftHand.SetTrigger("Unpause");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void AnimationTimer()
    {
        if (is_animating)
        {
            animationTimer += Time.deltaTime;
            bool paused = GameSystems.State?.IsPaused ?? false;
            if (!paused)
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
