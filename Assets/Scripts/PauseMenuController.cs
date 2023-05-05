using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public bool debug;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public static bool isPaused; // It's made static in order for other functions to know that the app is paused
    public bool isCursorLocked;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the menus on start
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);

        isCursorLocked = true;
        //Make the cursor invisible
        Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isCursorLocked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonPressDebug("Esc");
            if (!isPaused)
            {
                PauseSession();
                //Make the cursor visible
                isCursorLocked = false;
            }
            else
            {
                ResumeSession();
                //Make the cursor invisible
                isCursorLocked = true;
            }
            Cursor.lockState = isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLocked;
        }
    }

    public void PauseSession()
    {
        CustomDebug("Application paused");
        pauseMenu.SetActive(true);
        // Pause the app in the background
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeSession()
    {
        CustomDebug("Application resumed");
        pauseMenu.SetActive(false);
        // Resume the app
        Time.timeScale = 1.0f;
        isPaused = false;
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
        Time.timeScale = 1.0f;
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
        if (debug)
        {
            Debug.Log(message);
        }
    }

    private void ButtonPressDebug(string message)
    {
        if (debug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
