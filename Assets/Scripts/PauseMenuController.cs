using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonDebug;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    //
    // Other panels
    public GameObject itemPanel;
    public GameObject cartPanel;
    public GameObject settingsPanel;
    public GameObject sessionAlertPanel;

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
            // Only open pause menu if no panels are currently active
            if(!itemPanel.activeSelf && !cartPanel.activeSelf && !settingsPanel.activeSelf && !sessionAlertPanel.activeSelf)
            {
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

    public void Restart(){
        SceneManager.LoadScene("LoadingScreen");
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

    /* public void DestroyItems()
    {
        CustomDebug("Destroying items");
        // Iterate through all the child objects and destroy them
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
            CustomDebug("Destroyed item: " + child.gameObject);
        }
    } */

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
