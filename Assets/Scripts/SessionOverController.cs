using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionOverController : MonoBehaviour
{
    public bool buttonDebug;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    private void ButtonPressDebug(string message)
    {
        if (buttonDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
