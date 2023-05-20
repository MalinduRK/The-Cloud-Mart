using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SessionController : MonoBehaviour
{
    public bool buttonDebug;
    public Toggle sessionControlToggle;

    public void SetSessionControls()
    {
        // if toggle on
        if (sessionControlToggle.isOn)
        {
            ButtonPressDebug("Session Toggle on");
        }
        // if toggle off
        else
        {
            ButtonPressDebug("Session Toggle off");
        }
    }

    public void Back()
    {
        ButtonPressDebug("Back");
        SceneManager.LoadScene("MainMenu");
    }

    public void CreateSession()
    {
        ButtonPressDebug("Create Session");
        SceneManager.LoadScene("LoadingScreen");
    }

    private void ButtonPressDebug(string message)
    {
        if (buttonDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
