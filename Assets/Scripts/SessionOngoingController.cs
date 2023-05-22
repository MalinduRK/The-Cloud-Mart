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

    // Initialize budget variables
    private int budgetValue;

    // Initialize focus variables
    private string focusValue;

    // Session Alert Panel
    public GameObject SessionAlertPanel;
    public TextMeshProUGUI SessionAlertText;
    public GameObject ExtendSessionButton;
    public GameObject ExtendBudgetButton;
    public GameObject RealizeFocusButton;
    public GameObject ClosePanelButton;

    // Raycast
    public Camera mainCamera;
    public float maxDistance = 2.5f;

    // Other panels and objects
    public GameObject ItemDetailsPanel;

    // Other variables
    public static bool isSessionHalted;

    // Start is called before the first frame update
    void Start()
    {
        // The session is not halted at the start
        isSessionHalted = false;

        // Hide Cursor on session start
        GameState.HideCursor();

        // Hide Session Alert Panel on start
        SessionAlertPanel.SetActive(false);

        // Assign user controls to local variables
        hours = ControlsController.hourValue;
        minutes = ControlsController.minuteValue;
        budgetValue = ControlsController.budgetValue;

        //For testing:
        hours = 0;
        minutes = 0;
        seconds = 10;
        budgetValue = 500;
        focusValue = "Bed";

        // .ToString("00") makes the numbers in 2-digit format
        time.text = $"Session Time: {hours.ToString("00")}:{minutes.ToString("00")}:00";
        budget.text = $"Budget: ${budgetValue}";
        focus.text = $"Primary Focus: {focusValue}";

        // Start the coroutine to update seconds at regular intervals
        StartCoroutine(UpdateSeconds());
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

        // Open pop-up when time is up
        SessionAlert(1);
    }

    public void UpdateBudget()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            string objectName = hit.collider.gameObject.name;
            // Reduce budget based on item price
            if (MultiModelLoader.items.ContainsKey(objectName))
            {
                int price = (int)MultiModelLoader.items[objectName].ItemPrice;
                // Change budget only if price does not exceed the remaining budget
                if (budgetValue>=price)
                {
                    budgetValue -= price;
                }
                else
                {
                    SessionAlert(2);
                }
            }
        }

        // Update UI
        budget.text = $"Budget: ${budgetValue.ToString()}";
    }

    public void ExtendTime()
    {
        // Add 5 extra minutes
        minutes += 5;

        // Check if minutes exceeded 60 and convert to hours
        if(minutes>60)
        {
            hours = minutes/60;
            minutes = minutes%60;
        }

        // Update UI
        time.text = $"Session Time: {hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";

        EnableCloseButton();

        // Restart time countdown
        StartCoroutine(UpdateSeconds());
    }

    public void AddBudget()
    {
        // Add $500 extra budget
        budgetValue += 500;

        // Update UI
        budget.text = $"Budget: ${budgetValue.ToString()}";

        EnableCloseButton();
    }

    private void SessionAlert(int alert)
    {
        // 1: Time
        // 2: Budget
        // 3: Focus

        // Halt session
        isSessionHalted = true;

        string alertMessage = "";

        // Show session alert panel
        SessionAlertPanel.SetActive(true);
        // Hide all session related buttons from the panel
        ExtendSessionButton.SetActive(false);
        ExtendBudgetButton.SetActive(false);
        RealizeFocusButton.SetActive(false);
        ClosePanelButton.SetActive(false);
        // Hide Item Details Panel
        ItemDetailsPanel.SetActive(false);

        switch(alert)
        {
            case 1: 
            alertMessage = "Your time is up!";
            ExtendSessionButton.SetActive(true);
            break;

            case 2: 
            alertMessage = "Your budget is too low!";
            ExtendBudgetButton.SetActive(true);
            break;

            case 3: 
            alertMessage = "You are not in focus!";
            RealizeFocusButton.SetActive(true);
            break;
        }

        SessionAlertText.text = alertMessage;
        PauseSession();
    }

    private void EnableCloseButton()
    {
        ClosePanelButton.SetActive(true);
    }

    public void PauseSession()
    {
        GameState.ShowCursor();
        GameState.DisableCameraMovement();
        GameState.DisablePlayerMovement();
        GameState.PauseTime();
    }

    public void ResumeSession()
    {
        GameState.HideCursor();
        GameState.EnableCameraMovement();
        GameState.EnablePlayerMovement();
        GameState.ResumeTime();
    }

    public void ClosePanel()
    {
        SessionAlertPanel.SetActive(false);
    }

    private void CustomDebug(string message)
    {
        if (customDebug)
        {
            Debug.Log(message);
        }
    }
}
