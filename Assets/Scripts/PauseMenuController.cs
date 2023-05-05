using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonDebug;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the menus on start
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonPressDebug("Esc");
            if (!GameState.timeIsPaused)
            {
                PauseSession();
            }
            else
            {
                ResumeSession();
            }
        }
    }

    public void PauseSession()
    {
        // Pause session
        GameState.PauseTime();
        GameState.DisableCameraMovement();
        GameState.DisablePlayerMovement();
        // Make the cursor visible
        GameState.ShowCursor();
        // Show menu
        pauseMenu.SetActive(true);
    }

    public void ResumeSession()
    {
        // Resume session
        GameState.ResumeTime();
        GameState.EnableCameraMovement();
        GameState.EnablePlayerMovement();
        // Make the cursor invisible
        GameState.HideCursor();
        // Hide menu
        pauseMenu.SetActive(false);
    }

    public void Settings()
    {
        ButtonPressDebug("Settings");
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        ButtonPressDebug("Quit to Main Menu");
        CustomDebug("Loading");
        GameState.ResumeTime();
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
        ButtonPressDebug("Exit to desktop");
        Application.Quit();
    }
    
    // Settings section

    public void Back()
    {
        ButtonPressDebug("Back");
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    private void CustomDebug(string message)
    {
        if (customDebug)
        {
            Debug.Log(message);
        }
    }

    private void ButtonPressDebug(string message)
    {
        if (buttonDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
