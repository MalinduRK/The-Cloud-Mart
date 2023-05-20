using TMPro;
using UnityEngine;

public class SessionOngoingController : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI budget;
    public TextMeshProUGUI focus;

    // Start is called before the first frame update
    void Start()
    {
        time.text = $"Session Time: {ControlsController.hourValue}:{ControlsController.minuteValue}:00";
        budget.text = $"Budget: ${ControlsController.budgetValue}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
