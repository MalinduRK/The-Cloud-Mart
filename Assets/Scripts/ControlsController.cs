using UnityEngine;
using TMPro;    

public class ControlsController : MonoBehaviour
{
    public bool buttonDebug;
    // Time
    public TextMeshProUGUI hour;
    public TextMeshProUGUI minute;
    // Budget
    public TextMeshProUGUI budget;
    // Session values available anywhere (static)
    public static int hourValue;
    public static int minuteValue;
    public static int budgetValue;

    public void HourUp()
    {
        ButtonPressDebug("Hour Up");
        // Get hour value
        string hourString = hour.text;
        // Convert to int
        hourValue = int.Parse(hourString);
        // Increment hour value by 1
        hourValue++;
        // Change hour text in the UI
        hour.text = hourValue.ToString();
    }

    public void HourDown()
    {
        ButtonPressDebug("Hour Down");
        // Get hour value
        string hourString = hour.text;
        // Convert to int
        hourValue = int.Parse(hourString);
        // Decrement hour value by 1
        if (hourValue > 0)
        {
            hourValue--;
        }
        // Change hour text in the UI
        hour.text = hourValue.ToString();
    }

    public void MinuteUp()
    {
        ButtonPressDebug("Minute Up");
        // Get minute value
        string minuteString = minute.text;
        // Convert to int
        minuteValue = int.Parse(minuteString);
        // Increment minute value by 5
        if (minuteValue < 60)
        {
            minuteValue += 5;
        }
        // Change minute text in the UI
        minute.text = minuteValue.ToString();
    }

    public void MinuteDown()
    {
        ButtonPressDebug("Minute Down");
        // Get minute value
        string minuteString = minute.text;
        // Convert to int
        minuteValue = int.Parse(minuteString);
        // Decrement minute value by 5
        if (minuteValue > 0)
        {
            minuteValue -= 5;
        }
        // Change minute text in the UI
        minute.text = minuteValue.ToString();
    }

    public void BudgetUp()
    {
        ButtonPressDebug("Budget Up");
        // Get budget value
        string budgetString = budget.text;
        // Convert to int
        budgetValue = int.Parse(budgetString);
        // Increment budget value by 50
        if (budgetValue < 950)
        {
            budgetValue += 50;
        }
        // Change budget text in the UI
        budget.text = budgetValue.ToString();
    }

    public void BudgetDown()
    {
        ButtonPressDebug("Budget Down");
        // Get budget value
        string budgetString = budget.text;
        // Convert to int
        budgetValue = int.Parse(budgetString);
        // Decrement budget value by 50
        if (budgetValue > 0)
        {
            budgetValue -= 50;
        }
        // Change budget text in the UI
        budget.text = budgetValue.ToString();
    }

    public void ChangeFocus()
    {

    }

    private void ButtonPressDebug(string message)
    {
        if (buttonDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
