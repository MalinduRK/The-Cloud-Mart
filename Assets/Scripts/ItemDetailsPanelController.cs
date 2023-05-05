using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsPanelController : MonoBehaviour
{
    public GameObject itemDetailsPanel;
    public Text promptText;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the panel on start
        itemDetailsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClosePanel()
    {
        // Disable mouse
        GameState.HideCursor();
        // Enable camera and player movement
        GameState.EnableCameraMovement();
        GameState.EnablePlayerMovement();
        // Show button prompt when closing the panel
        promptText.text = "View details\n[Left Mouse Button]";
        itemDetailsPanel.SetActive(false);
    }
}
