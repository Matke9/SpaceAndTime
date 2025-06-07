using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    public static bool IsPause = false;

    public GameObject PauseMenuUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        void Resume()
        {
            PauseMenuUI.SetActive(false);
            Time.timeScale = 1.0f;
            IsPause = false;
        }
        void Pause()
        {
            PauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            IsPause = true;
        }
    }
}
