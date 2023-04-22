using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
