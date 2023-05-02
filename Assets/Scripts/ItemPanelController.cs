using UnityEngine;
using UnityEngine.UI;

public class ItemPanelController : MonoBehaviour
{
    public Button backButton;
    public Button addToCartButton;
    // Item panel
    public GameObject itemPanel;
    public Text promptText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && MultiModelLoader.isItemPanelOpen)
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        // Disable the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Unlock player movement
        PauseMenu.isPaused = false;

        promptText.text = "View details\n[Left Mouse Button]";
        itemPanel.SetActive(false);
        MultiModelLoader.isItemPanelOpen = false;
    }
}
