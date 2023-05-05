using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CartPanelController : MonoBehaviour
{
    public bool customDebug;
    public bool buttonDebug;
    public GameObject cartMenu;
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
            if (!isCartPanelOpen)
            {
                OpenPanel();
            }
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

        // Read item Ids from cart list
        List<string> cart = ItemDetailsPanelController.cart;

        // Instantiate new object from CartItem model
        cartItems = new List<CartItem>();

        // Instantiate item prefabs and populate with data
        for (int i = 0; i < cart.Count; i++)
        {
            string name = "";
            string seller = "";
            string price = "";
            string imageExtension = "";

            // Retrieve required values from the items dictionary
            if (MultiModelLoader.items.ContainsKey(cart[i]))
            {
                name = MultiModelLoader.items[cart[i]].ItemName;
                seller = MultiModelLoader.items[cart[i]].SellerName;
                price = MultiModelLoader.items[cart[i]].ItemPrice.ToString();
                imageExtension = MultiModelLoader.items[cart[i]].ImageFileExtension;
            }

            // Create sprite using image name and extension
            //
            //
            string fileName = cart[i] + "_image." + imageExtension;
            string imagePath = $"{Application.persistentDataPath}/Files/Images/";
            //
            // Read the image data from file
            byte[] imageData = File.ReadAllBytes(imagePath + fileName);
            //
            // Create a new texture and load the image data into it
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            //
            // Create a new sprite from the texture and set it on the Image UI element
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Add items to the new list
            cartItems.Add(new CartItem(name, seller, price, sprite));

            GameObject item = Instantiate(itemPrefab, cartContent);
            CartItemLayout layout = item.GetComponent<CartItemLayout>();
            layout.Populate(cartItems[i]);
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

    public CartItem(string name, string seller, string price, Sprite image)
    {
        this.name = name;
        this.seller = seller;
        this.price = price;
        this.image = image;
    }
}