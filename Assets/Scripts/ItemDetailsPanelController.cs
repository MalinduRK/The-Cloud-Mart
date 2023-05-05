using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsPanelController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonPressDebug;
    // Object references
    public GameObject itemDetailsPanel;
    public Text promptText;
    public TextMeshProUGUI cartText;
    List<string> cart = new List<string>();
    // Raycast
    public Camera mainCamera;
    public float maxDistance = 2.5f;

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
        ButtonPressDebug("Back");
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
        // Raycast
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            string objectName = hit.collider.gameObject.name;
            cart.Add(objectName);
            CustomDebug($"{objectName} added to cart");
        }
        cartText.text = $"In Cart: {cart.Count}";
    }

    private void CustomDebug(string message)
    {
        if (customDebug)
        {
            Debug.Log(message);
        }
    }

    private void ButtonPressDebug(string message)
    {
        if (customDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}
