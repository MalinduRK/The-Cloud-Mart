using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsPanelController : MonoBehaviour
{
    // Object references
    public GameObject itemDetailsPanel;
    public Text promptText;
    public TextMeshProUGUI cartText;
    List<string> cart = new List<string>();

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

    public void AddToCart()
    {
        cart.Add("test");
        cartText.text = $"In Cart: {cart.Count}";
    }
}
