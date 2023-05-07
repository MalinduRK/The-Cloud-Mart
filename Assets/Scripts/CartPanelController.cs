using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class CartPanelController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonDebug;
    public GameObject cartMenu;
    public TextMeshProUGUI cartText;
    public GameObject itemPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    // Scroll view
    public GameObject itemPrefab;
    public RectTransform cartContent;
    private List<CartItem> cartItems;       // list of items to display
    //
    private bool isCartPanelOpen;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the menus on start
        cartMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ButtonPressDebug("Return");
            if (!isCartPanelOpen && !itemPanel.activeSelf && !pausePanel.activeSelf && !settingsPanel.activeSelf)
            {
                OpenPanel();
            }
            // Reading the cart again since there was an issue with loading. It works correctly only when this is done, until the solution is found
            ReadCart();
        }
    }

    public void OpenPanel()
    {
        isCartPanelOpen = true;
        ReadCart();
        // Enable mouse
        GameState.ShowCursor();
        // Disable camera and player movement
        GameState.DisableCameraMovement();
        GameState.DisablePlayerMovement();
        cartMenu.SetActive(true);
    }

    public void ClosePanel()
    {
        isCartPanelOpen = false;
        ButtonPressDebug("Back");
        // Disable mouse
        GameState.HideCursor();
        // Enable camera and player movement
        GameState.EnableCameraMovement();
        GameState.EnablePlayerMovement();
        cartMenu.SetActive(false);
    }

    public void ReadCart()
    {
        CustomDebug("Reading cart");

        // Clear previous items in the cart
        foreach (Transform child in cartContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Read items and quantities from the cart dictionary
        Dictionary<string, int> cart = ItemDetailsPanelController.cart;

        // Instantiate new object from CartItem model
        cartItems = new List<CartItem>();

        // Instantiate item prefabs and populate with data
        foreach (KeyValuePair<string, int> cartItem in cart)
        {
            string itemId = cartItem.Key;
            int quantity = cartItem.Value;

            string name = "";
            string seller = "";
            string price = "";
            string imageExtension = "";

            // Retrieve required values from the items dictionary
            if (MultiModelLoader.items.ContainsKey(itemId))
            {
                Debug.Log($"MultiModelLoader contains {itemId}");
                name = MultiModelLoader.items[itemId].ItemName;
                seller = MultiModelLoader.items[itemId].SellerName;
                price = MultiModelLoader.items[itemId].ItemPrice.ToString();
                imageExtension = MultiModelLoader.items[itemId].ImageFileExtension;
            }

            // Create sprite using image name and extension
            string fileName = itemId + "_image." + imageExtension;
            string imagePath = $"{Application.persistentDataPath}/Files/Images/";
            byte[] imageData = File.ReadAllBytes(imagePath + fileName);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Add items to the new list with quantity
            cartItems.Add(new CartItem(name, seller, price, sprite, quantity));

            GameObject item = Instantiate(itemPrefab, cartContent);
            if (itemPrefab.TryGetComponent<CartItemLayout>(out CartItemLayout layout))
            {
                CustomDebug("Populating items");
                layout.Populate(cartItems[cartItems.Count - 1]);
            }
            else
            {
                CustomDebug("Cannot populate items");
            }
        }
    }

    public void ClearCart()
    {
        ButtonPressDebug("Clear Cart");
        CustomDebug("Clearing cart");

        // Clear the cart list
        ItemDetailsPanelController.cart.Clear();

        // Clear the cart from the UI
        foreach (Transform child in cartContent.transform)
        {
            Destroy(child.gameObject);
        }

        cartText.text = "In Cart: -";
    }

    public void GoToCheckout()
    {
        string url = "https://thecloudmart.000webhostapp.com/checkout.html";
        // Open the specified URL in the default browser
        Application.OpenURL(url);
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
        if (buttonDebug)
        {
            Debug.Log("Button pressed: " + message);
        }
    }
}

public class CartItem
{
    public string name;
    public string seller;
    public string price;
    public Sprite image;
    public int quantity;

    public CartItem(string name, string seller, string price, Sprite image, int quantity)
    {
        this.name = name;
        this.seller = seller;
        this.price = price;
        this.image = image;
        this.quantity = quantity;
    }
}