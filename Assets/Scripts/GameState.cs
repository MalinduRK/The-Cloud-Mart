using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static bool timeIsPaused;
    public static bool cameraIsDisabled;
    public static bool movementIsDisabled;

    public static void PauseTime()
    {
        timeIsPaused = true;
    }

    public static void ResumeTime()
    {
        timeIsPaused = false;
    }
}
