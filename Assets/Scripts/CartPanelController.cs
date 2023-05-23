using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CartPanelController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonDebug;
    public GameObject cartMenu;
    public TextMeshProUGUI cartText;
    public GameObject itemPanel;
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject sessionAlertPanel;
    // Scroll view
    public GameObject itemPrefab;
    public RectTransform cartContent;
    private List<CartItem> cartItems;       // list of items to display
    Dictionary<string, int> cart = new Dictionary<string, int>();
    //
    private bool isCartPanelOpen;
    //
    // Firestore
    public string databaseURL; // The URL of your Firestore database
    public string collectionName; // The name of the collection to add the document to
    public string apiKey; // Your Firebase project's API key
    public string documentID; // The ID of the new Firestore document
    public string jsonData; // The data to be added to the new Firestore document, in JSON format

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
            if (!isCartPanelOpen && !itemPanel.activeSelf && !pausePanel.activeSelf && !settingsPanel.activeSelf && !sessionAlertPanel.activeSelf)
            {
                OpenPanel();
            }
            // Reading the cart again since there was an issue with loading. It works correctly only when this is done, until the solution is found
            ReadCart();
        }
    }

    async void Awake()
	{
		try
		{
			await UnityServices.InitializeAsync();
		}
		catch (Exception e)
		{
			Debug.LogException(e);
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
        // Pause session
        GameState.PauseTime();
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
        // Resume Session
        GameState.ResumeTime();
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
        cart = ItemDetailsPanelController.cart;

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
                price = (MultiModelLoader.items[itemId].ItemPrice*quantity).ToString();
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
        // Wrap your dictionary in a wrapper class
        //DictionaryWrapper wrapper = new DictionaryWrapper(cart);

        // Convert the wrapper class to JSON format
        //jsonData = JsonUtility.ToJson(wrapper);

        // Output the JSON string
        //Debug.Log(jsonData);

        // Post the data into firestore
        PostToFirestore();
        string url = "https://thecloudmart.000webhostapp.com/checkout.html";
        // Open the specified URL in the default browser
        Application.OpenURL(url);

        // End session inside the application
        SceneManager.LoadScene("SessionOverScreen");
    }

    /* // Wrapper class for dictionary
    [System.Serializable]
    private class DictionaryWrapper
    {
        public Dictionary<string, int> dictionary;

        public DictionaryWrapper(Dictionary<string, int> dict)
        {
            dictionary = dict;
        }
    } */

    public void PostToFirestore()
    {
        // Debug.Log(AuthenticationService.Instance.PlayerId);

        // Create a new UnityWebRequest object
        UnityWebRequest www = new UnityWebRequest(databaseURL + "/" + collectionName + "/" + documentID, "POST");

        // Set the headers for the request
        //www.SetRequestHeader("Content-Type", "application/json");
        //www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Setting the headers doesn't work for some reason

        // Manually setting the json file
        jsonData = "{ \"fields\": {";
        jsonData += $"\"userId\": {{ \"stringValue\": \"{AuthenticationService.Instance.PlayerId}\"}},";
        foreach (KeyValuePair<string, int> cartItem in cart)
        {
            jsonData += $"\"{cartItem.Key}\": {{ \"stringValue\": \"{cartItem.Value}\"}},";
        }
        jsonData += "} }";

        // Set the data to be sent with the request
        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(data);
        www.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        StartCoroutine(SendRequest(www));
    }

    IEnumerator SendRequest(UnityWebRequest www)
    {
        // Wait for the response from the server
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Cart added to Firestore!");
        }
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