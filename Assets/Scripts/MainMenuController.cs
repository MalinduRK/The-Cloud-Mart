using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Enter()
    {
        // Can use this to load next scene in the Build Settings
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void GoToSettingsMenu()
    {
        SceneManager.LoadScene("MainMenuSettings");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
