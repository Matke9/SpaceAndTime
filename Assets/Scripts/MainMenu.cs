using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadSceneAsync(0);

    }

    public void quitGame()
    { 
         Application.Quit();
        Debug.Log("Quit");
    }

}
