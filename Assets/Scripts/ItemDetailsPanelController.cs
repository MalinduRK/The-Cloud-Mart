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
    public static Dictionary<string, int> cart = new Dictionary<string, int>();
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
            if (cart.ContainsKey(objectName))
            {
                cart[objectName] += 1; // increase the count by 1 if the item already exists in the cart
            }
            else
            {
                cart.Add(objectName, 1); // add the item to the cart with a count of 1 if it doesn't exist in the cart
            }
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
