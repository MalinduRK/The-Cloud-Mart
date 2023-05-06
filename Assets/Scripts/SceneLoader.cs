using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName; // The name of the scene to load
    public float delayTime; // The delay time in seconds

    void Start()
    {
        // Call the LoadScene() method after a delay
        Invoke("LoadScene", delayTime);
    }

    void LoadScene()
    {
        // Load the specified scene
        SceneManager.LoadScene(sceneName);
    }
}