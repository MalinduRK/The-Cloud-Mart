using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool debug;
    public static bool timeIsPaused;
    public static bool cameraIsDisabled;
    public static bool movementIsDisabled;
    public static bool cursorIsHidden;

    public static void PauseTime()
    {
        timeIsPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Application paused");
    }

    public static void ResumeTime()
    {
        timeIsPaused = false;
        Time.timeScale = 1.0f;
        Debug.Log("Application resumed");
    }

    public static void DisableCameraMovement()
    {
        cameraIsDisabled = true;
        Debug.Log("Camera movement disabled");
    }

    public static void EnableCameraMovement()
    {
        cameraIsDisabled = false;
        Debug.Log("Camera movement enabled");
    }

    public static void DisablePlayerMovement()
    {
        movementIsDisabled = true;
        Debug.Log("Player movement disabled");
    }

    public static void EnablePlayerMovement()
    {
        movementIsDisabled = false;
        Debug.Log("Player movement enabled");
    }

    public static void HideCursor()
    {
        cursorIsHidden = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked");
    }

    public static void ShowCursor()
    {
        cursorIsHidden = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor unlocked");
    }
}
