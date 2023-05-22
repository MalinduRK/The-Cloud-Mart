using System.Collections;
using TMPro;
using UnityEngine;

public class SessionOngoingController : MonoBehaviour
{
    public bool customDebug;
    public TextMeshProUGUI time;
    public TextMeshProUGUI budget;
    public TextMeshProUGUI focus;
    // Initialize time variables
    private int hours;
    private int minutes;
    private int seconds;

    // Start is called before the first frame update
    void Start()
    {
        // Assign user selected times to the local variables
        hours = ControlsController.hourValue;
        minutes = ControlsController.minuteValue;

        //For testing:
        hours = 0;
        minutes = 0;
        seconds = 10;

        // .ToString("00") makes the numbers in 2-digit format
        time.text = $"Session Time: {hours.ToString("00")}:{minutes.ToString("00")}:00";
        budget.text = $"Budget: ${ControlsController.budgetValue}";
        focus.text = $"Primary Focus: {ControlsController.focusValue}";

        // Start the coroutine to update seconds at regular intervals
        StartCoroutine(UpdateSeconds());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator UpdateSeconds()
    {
        bool timeIsUp = false;
        while (!timeIsUp)
        {
            // Decrease seconds value by one every second
            seconds--;

            if (seconds < 0)
            {
                // Decrease minutes value by one if seconds reach zero
                minutes--;
                seconds = 59;

                if (minutes < 0)
                {
                    // Decrease hours value by one if minutes reach zero
                    hours--;
                    minutes = 59;

                    if (hours < 0)
                    {
                        // Assign zero to all values when time is up
                        hours = 0;
                        minutes = 0;
                        seconds = 0;
                        // Time's up, perform any necessary actions
                        // You can stop the countdown here or reset the timer as needed
                        CustomDebug("Time's up!");
                        timeIsUp = true;
                    }
                }
            }

            // Update the TextMeshPro Text object with the new values
            time.text = $"Session Time: {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";

            // Wait for 1 second
            yield return new WaitForSeconds(1f);
        }
    }

    private void CustomDebug(string message)
    {
        if (customDebug)
        {
            Debug.Log(message);
        }
    }
}
