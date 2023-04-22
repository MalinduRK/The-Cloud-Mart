using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public static bool isPaused; // It's made static in order for other functions to know that the app is paused

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
            if (!isPaused)
            {
                PauseSession();
                //Set Cursor to not be visible
                Cursor.visible = true;
            }
            else
            {
                ResumeSession();
            }
        }
    }

    public void PauseSession()
    {
        pauseMenu.SetActive(true);
        // Pause the app in the background
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeSession()
    {
        pauseMenu.SetActive(false);
        // Resume the app
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
    
    // Settings section

    public void Back()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
}
