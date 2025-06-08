using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadSceneAsync("Level1");

    }

    public void quitGame()
    { 
         Application.Quit();
        Debug.Log("Quit");
    }

}
