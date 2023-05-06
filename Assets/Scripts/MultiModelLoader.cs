using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Siccity.GLTFUtility;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

[System.Serializable]
public class Item
{
    public string name;
    public string bucket;
}

[System.Serializable]
public class ItemList
{
    public string[] prefixes;
    public Item[] items;
}

public class MultiModelLoader : MonoBehaviour
{
    public bool customDebug;
    public bool buttonPressDebug;
    public string storageUrl = "https://firebasestorage.googleapis.com/v0/b/the-cloud-mart.appspot.com/o/";
    public string bucketPath = "models/";
    public string apiKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC7KFInf0JYb+/Q";
    // File paths
    //
    public string localPath;
    public string imagePath;
    public string modelPath;
    public string category;
    // Create a parent object to hold all the downloaded objects
    public GameObject modelLoaderObject;
    public GameObject parentObject;
    public GameObject baseObject;
    // Create a dictionary to store the items in separate, dynamically created arrays
    Dictionary<string, string[]> pages = new Dictionary<string, string[]>();
    // Keep track of page number in display
    private int pageNumber = 0;
    private int totalPages = 0;
    // Loading models
    //
    private GameObject gltfObject;
    public bool inverseLoad;
    // Loading more items
    //
    public Text promptText;
    public Camera mainCamera;
    // Distance where the text prompt is triggered
    public float maxDistance = 2.5f;
    public GameObject buttonObject;
    // Firestore data
    //
    public ItemDataStore dataStore;
    // Dictionary for storing all firestore data with the respective item ID
    public static Dictionary<string, ItemDataStore> items = new Dictionary<string, ItemDataStore>();
    private bool firestoreDataLoaded = false;
    // Item panel
    //
    public GameObject itemPanel;
    // Check if the item panel is open
    public bool isItemPanelOpen;
    public TextMeshProUGUI itemName; // Using TextMeshPro works only for 3D rendered texts
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI sellerName;
    public TextMeshProUGUI itemLength;
    public TextMeshProUGUI itemWidth;
    public TextMeshProUGUI itemHeight;
    public Image itemImage;

    void Start()
    {
        ItemDataLoader firestoreReader = FindObjectOfType<ItemDataLoader>();
        // Subscribe to the loader event from ItemDataLoader
        if (firestoreReader != null)
        {
            //firestoreReader.OnDocumentLoaded += DocumentLoadedCallback;
        }

        localPath = $"{Application.persistentDataPath}/Files/";
        imagePath = localPath + "Images/";
        modelPath = localPath + "Models/";

        ReadData();

        // Disable item panel on start
        //itemPanel.SetActive(false);
    }

    private void Update()
    {
        ItemDataLoader firestoreReader = FindObjectOfType<ItemDataLoader>();
        // Check if the firestore data had loaded up and is ready
        if (firestoreDataLoaded && dataStore != null)
        {
            //firestoreReader.OnDocumentLoaded += DocumentLoadedCallback;
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Only work if the app isn't paused
        if (!GameState.timeIsPaused)
        {
            // Check if the player camera is looking at the said object
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) && hit.collider.gameObject == buttonObject)
            {
                promptText.text = "Load more items\n[Left Mouse Button]\n(" + pageNumber + "/" + totalPages + ")";
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    ButtonPressDebug("Left Mouse Button");
                    StartCoroutine(LoadFiles());
                }
            }
            // Check if the player is looking at a loaded item and take action
            else if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name == "Items")
            {
                if (!isItemPanelOpen)
                {
                    promptText.text = "View details\n[Left Mouse Button]";
                }
                
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    ButtonPressDebug("Left Mouse Button");
                    if (!isItemPanelOpen)
                    {
                        EnterPanel();
                        // Hide button prompt when the panel is open
                        promptText.text = "";
                        // Get the name of the object that was clicked
                        string objectName = hit.collider.gameObject.name;
                        ButtonPressDebug("Object: " + objectName);

                        // Find the dictionary item that aligns with the game object
                        if (items.ContainsKey(objectName))
                        {
                            // Display item values
                            itemName.text = items[objectName].ItemName;
                            itemDescription.text = items[objectName].ItemDescription;
                            itemPrice.text = items[objectName].ItemPrice.ToString();
                            sellerName.text = items[objectName].SellerName;
                            itemLength.text = items[objectName].ItemLength.ToString();
                            itemWidth.text = items[objectName].ItemWidth.ToString();
                            itemHeight.text = items[objectName].ItemHeight.ToString();
                            StartCoroutine(ShowImage(objectName));
                        }

                        // Open item panel
                        itemPanel.SetActive(true);
                        isItemPanelOpen = true;
                    }
                }
            }
            else
            {
                promptText.text = "";
                itemPanel.SetActive(false);
                isItemPanelOpen = false;
            }
        }
    }

    private void ReadData()
    {
        string filePath = $"{Application.persistentDataPath}/Files/json/{category}/{category}.json"; // Replace with the file path where you saved the JSON file earlier

        // Read the contents of the JSON file
        string jsonContent = File.ReadAllText(filePath);

        // Parse the JSON content into a JObject
        JObject jsonObject = JObject.Parse(jsonContent);
        JArray documents = (JArray)jsonObject["documents"];

        // Initialize an array of document ids
        string[] docIdArray = new string[0];
        int arraySize = 0;
        int counter = 0;

        foreach (JToken document in documents)
        {
            // Extract field values from the "fields" JObject
            JObject fields = (JObject)document["fields"];
            // Get the modelAdded boolean value from the data
            JToken modelAddedToken = fields["modelAdded"];
            if (modelAddedToken != null)
            {
                string modelAdded = modelAddedToken["booleanValue"].ToString();
                //Debug.Log("Value of modelAdded variable: " + modelAdded);

                if (modelAdded == "True")
                {
                    //Debug.Log("Model added");
                    // Increase size of array by 1 for each item that is viable
                    Array.Resize(ref docIdArray, ++arraySize);

                    // Get the filepath of the document in firebase
                    string documentUrl = document["name"].ToString();

                    // Split the path into an array to isolate the document id
                    string[] urlSplitArray = documentUrl.Split('/');

                    // Take the array size in order to read the last item in the array, which is the document id
                    int urlSplitArraySize = urlSplitArray.Length;

                    // Put the document id into a variable
                    string documentId = urlSplitArray[urlSplitArraySize - 1];
                    //CustomDebug(documentId);

                    // Add the document id to the array to be sent to the datastore
                    docIdArray[counter++] = documentId;


                    // Put all data into separate variables
                    string itemName = fields["itemName"]["stringValue"].ToString();
                    string itemDescription = fields["itemDescription"]["stringValue"].ToString();
                    float itemPrice = (float)fields["itemPrice"]["integerValue"];
                    string sellerName = fields["sellerName"]["stringValue"].ToString();

                    /*
                    float itemLength = 0.0f;
                    float itemWidth = 0.0f;
                    float itemHeight = 0.0f;
                    // Add database values only if none of the dimensions are null
                    if (fields["itemLength"]["integerValue"] != null && fields["itemWidth"]["integerValue"] != null && fields["itemHeight"]["integerValue"] != null)
                    {
                        itemLength = (float)fields["itemLength"]["integerValue"];
                        itemWidth = (float)fields["itemWidth"]["integerValue"];
                        itemHeight = (float)fields["itemHeight"]["integerValue"];
                    }
                    */

                    float itemLength = (float)fields["itemLength"]["integerValue"];
                    float itemWidth = (float)fields["itemWidth"]["integerValue"];
                    float itemHeight = (float)fields["itemHeight"]["integerValue"];

                    string imageFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["imageFileExtension"]["stringValue"] != null)
                    {
                        imageFileExtension = fields["imageFileExtension"]["stringValue"].ToString();
                    }

                    string modelFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["modelFileExtension"]["stringValue"] != null)
                    {
                        modelFileExtension = fields["modelFileExtension"]["stringValue"].ToString();
                    }

                    // Add all the document data to the items dictionary, using the data model class ItemDataStore
                    items.Add(documentId, new ItemDataStore
                    {
                        ItemName = itemName,
                        ItemDescription = itemDescription,
                        ItemPrice = itemPrice,
                        SellerName = sellerName,
                        ItemLength = itemLength,
                        ItemWidth = itemWidth,
                        ItemHeight = itemHeight,
                        ImageFileExtension = imageFileExtension,
                        ModelFileExtension = modelFileExtension
                    });
                }
            }
        }

        if (docIdArray.Length > 0)
        {
            CustomDebug($"Number of documents: {docIdArray.Length}");
            PaginateItems(docIdArray);
        }
    }

    private void DocumentLoadedCallback(JArray documents)
    {
        // Set the flag to indicate that the Firestore data has been loaded
        firestoreDataLoaded = true;

        // Initialize an array of document ids
        string[] docIdArray = new string[0];
        int arraySize = 0;
        int counter = 0;

        foreach (JToken document in documents)
        {
            // Extract field values from the "fields" JObject
            JObject fields = (JObject)document["fields"];
            // Get the modelAdded boolean value from the data
            JToken modelAddedToken = fields["modelAdded"];
            if (modelAddedToken != null)
            {
                string modelAdded = modelAddedToken["booleanValue"].ToString();
                //Debug.Log("Value of modelAdded variable: " + modelAdded);

                if (modelAdded == "True")
                {
                    //Debug.Log("Model added");
                    // Increase size of array by 1 for each item that is viable
                    Array.Resize(ref docIdArray, ++arraySize);

                    // Get the filepath of the document in firebase
                    string documentUrl = document["name"].ToString();

                    // Split the path into an array to isolate the document id
                    string[] urlSplitArray = documentUrl.Split('/');

                    // Take the array size in order to read the last item in the array, which is the document id
                    int urlSplitArraySize = urlSplitArray.Length;

                    // Put the document id into a variable
                    string documentId = urlSplitArray[urlSplitArraySize - 1];
                    //CustomDebug(documentId);

                    // Add the document id to the array to be sent to the datastore
                    docIdArray[counter++] = documentId;


                    // Put all data into separate variables
                    string itemName = fields["itemName"]["stringValue"].ToString();
                    string itemDescription = fields["itemDescription"]["stringValue"].ToString();
                    float itemPrice = (float)fields["itemPrice"]["integerValue"];
                    string sellerName = fields["sellerName"]["stringValue"].ToString();

                    /*
                    float itemLength = 0.0f;
                    float itemWidth = 0.0f;
                    float itemHeight = 0.0f;
                    // Add database values only if none of the dimensions are null
                    if (fields["itemLength"]["integerValue"] != null && fields["itemWidth"]["integerValue"] != null && fields["itemHeight"]["integerValue"] != null)
                    {
                        itemLength = (float)fields["itemLength"]["integerValue"];
                        itemWidth = (float)fields["itemWidth"]["integerValue"];
                        itemHeight = (float)fields["itemHeight"]["integerValue"];
                    }
                    */

                    float itemLength = (float)fields["itemLength"]["integerValue"];
                    float itemWidth = (float)fields["itemWidth"]["integerValue"];
                    float itemHeight = (float)fields["itemHeight"]["integerValue"];

                    string imageFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["imageFileExtension"]["stringValue"] != null)
                    {
                        imageFileExtension = fields["imageFileExtension"]["stringValue"].ToString();
                    }

                    string modelFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["modelFileExtension"]["stringValue"] != null)
                    {
                        modelFileExtension = fields["modelFileExtension"]["stringValue"].ToString();
                    }

                    // Add all the document data to the items dictionary, using the data model class ItemDataStore
                    items.Add(documentId, new ItemDataStore{ 
                        ItemName = itemName,
                        ItemDescription = itemDescription,
                        ItemPrice = itemPrice,
                        SellerName = sellerName,
                        ItemLength = itemLength,
                        ItemWidth = itemWidth,
                        ItemHeight = itemHeight,
                        ImageFileExtension = imageFileExtension,
                        ModelFileExtension = modelFileExtension
                    });
                }
            }
        }

        if (docIdArray.Length > 0)
        {
            CustomDebug($"Number of documents: {docIdArray.Length}");
            PaginateItems(docIdArray);
        }
    }

    void PaginateItems(string[] itemName)
    {
        //Debug.Log("Length of itemName array: " + itemName.Length);
        // Count the added items to create a proper array
        int itemCount = 0;

        // Calcualte the number of pages needed
        double result = (double)itemName.Length / 4;
        totalPages = (int)Math.Ceiling(result);

        // Use a for loop to create the variables and add them to the dictionary
        for (int i = 0; i < totalPages; i++)
        {
            // Define the name of the variable
            string variableName = "page" + (i+1).ToString();
            string[] variableValue = new string[4];

            for (int j = 0; j <4; j++)
            {
                // Prevent the array from going out of bounds
                if (j+itemCount+1 <= itemName.Length)
                {
                    //Debug.Log("Name of item " + (j+itemCount) + itemName[j + itemCount]);
                    variableValue[j] = itemName[j+itemCount];
                }
                // Prevent from further looping when the array is completed
                else
                {
                    // Resize array to match the number of items in the page
                    Array.Resize(ref variableValue, j);
                    break;
                }
            }
            itemCount += 4;
            CustomDebug("Iteration " + i + " complete");

            // Add the variable to the dictionary
            pages.Add(variableName, variableValue);
        }
        // Call the LoadFiles function to load in the first four items at the start
        StartCoroutine(LoadFiles());
        /*
        Debug.Log(pages["page1"][0]);
        Debug.Log(pages["page1"][1]);
        Debug.Log(pages["page1"][2]);
        Debug.Log(pages["page1"][3]);
        */
    }

    public IEnumerator LoadFiles()
    {
        DestroyItems();
        // Pages should flip only up to the maximum number of pages
        if (pageNumber < totalPages)
        {
            CustomDebug("Flip page");
            pageNumber++;
        }
        // Otherwise, reset to the first page
        else
        {
            pageNumber = 1;
        }

        CustomDebug("Loading files");

        // Get the page number as the dictionary key
        string page = "page" + pageNumber;

        // Set initial position for the first game object
        Vector3 loaderPosition = parentObject.transform.position;
        //Debug.Log(loaderPosition);

        // Set the spacing for each object
        float spacing = 2.5f;

        for (int i = 0; i < pages[page].Length; i++)
        {
            //Debug.Log("Page length: " + pages[page].Length);
            // Counter to get the positions of loaded items
            int counter = i + 1;

            // Initialize object position as the position of the loader to make relative adjustments
            Vector3 objectPosition = loaderPosition;

            // Change positioning if the items are loaded on the left side of the mart
            if (inverseLoad)
            {
                switch (counter)
                {
                    case 1:
                        objectPosition += new Vector3(spacing, 0, spacing);
                        break;
                    case 2:
                        objectPosition += new Vector3(spacing, 0, -spacing);
                        break;
                    case 3:
                        objectPosition += new Vector3(-spacing, 0, spacing);
                        break;
                    case 4:
                        objectPosition += new Vector3(-spacing, 0, -spacing);
                        break;
                    default:
                        Debug.Log("Wrong counter in switch statement!");
                        yield break;
                }
            }
            else
            {
                switch (counter)
                {
                    case 1:
                        objectPosition += new Vector3(-spacing, 0, -spacing);
                        break;
                    case 2:
                        objectPosition += new Vector3(-spacing, 0, spacing);
                        break;
                    case 3:
                        objectPosition += new Vector3(spacing, 0, -spacing);
                        break;
                    case 4:
                        objectPosition += new Vector3(spacing, 0, spacing);
                        break;
                    default:
                        Debug.Log("Wrong counter in switch statement!");
                        yield break;
                }
            }

            StartCoroutine(DownloadAndSaveFile(pages[page][i], objectPosition));
        }
    }

    void DestroyItems()
    {
        CustomDebug("Destroying items");
        // Iterate through all the child objects and destroy them
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
            CustomDebug("Destroyed item: " + child.gameObject);
        }
    }

    IEnumerator DownloadAndSaveFile(string fileId, Vector3 position)
    {
        string extension = items[fileId].ModelFileExtension;
        //Debug.Log(extension);
        string fileName = fileId + "_model." + extension;
        //Debug.Log(fileName);
        string folderPath = modelPath + category + "/";
        string filePath = folderPath + fileName;

        // Check if the file already exists
        if (File.Exists(filePath))
        {
            CustomDebug("File " + fileName + " already exists in " + folderPath);
        }
        else
        {
            // Download the file from Firebase Storage and save it to local storage
            UnityWebRequest www = UnityWebRequest.Get(storageUrl + "models%2F" + fileName + "?alt=media");
            www.downloadHandler = new DownloadHandlerFile(filePath);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                yield break;
            }

            CustomDebug("File " + fileName + " downloaded and saved to " + folderPath);
        }

        // Load the GLTF file using the GLTFUtility library
        gltfObject = Importer.LoadFromFile(filePath);

        // Set the object's name to the file name (without extension)
        gltfObject.name = Path.GetFileNameWithoutExtension(fileId);

        // Set the parent object
        gltfObject.transform.SetParent(parentObject.transform);

        // Calculate the bounding box of the model before scaling
        Bounds preBounds = gltfObject.GetComponentInChildren<MeshRenderer>().bounds;

        // Calculate the scale factor needed to fit the bounding box within a desired size
        float desiredSize = 2f; // desired size in world units
        float scaleFactor = desiredSize / preBounds.size.magnitude;

        // Apply the scale factor to the model
        gltfObject.transform.localScale *= scaleFactor;

        // Add a BoxCollider to the object
        gltfObject.AddComponent<BoxCollider>();

        // Position the object in the scene as desired
        gltfObject.transform.position = position;

        // Add gravity to the objects and let it place itself on the surface
        gltfObject.AddComponent<Rigidbody>();
        gltfObject.GetComponent<Rigidbody>().isKinematic = false;
        gltfObject.GetComponent<Rigidbody>().useGravity = true;

        // Rotate the object as desired
        // gltfObject.transform.rotation = Quaternion.identity;

        /*
        // Attach a TextMeshPro component to the game object
        TextMeshPro textMeshPro = gltfObject.AddComponent<TextMeshPro>();

        // Set the text of the TextMeshPro component
        textMeshPro.text = $"Item Name: {items[fileId].ItemName}\nItem Description: {items[fileId].ItemDescription}\nItem Price: {items[fileId].ItemPrice}";
        */
    }

    IEnumerator ShowImage(string fileId)
    {
        if (items[fileId].ImageFileExtension == "")
        {
            //Debug.Log("Loading placeholder image");
            // Refer the image that should display when a seller has not added any image
            Sprite noImageSprite = Resources.Load<Sprite>("Images/NoImage.jpg");
            // Set image
            itemImage.sprite = noImageSprite;
            yield break; // The function will stop here if there is no image
        }

        Debug.Log("Loading image");

        string extension = items[fileId].ImageFileExtension;
        //Debug.Log(extension);
        string fileName = fileId + "_image." + extension;
        //Debug.Log(fileName);
        string filePath = imagePath + fileName;

        // Check if the file already exists
        if (File.Exists(filePath))
        {
            CustomDebug("File " + fileName + " already exists in " + imagePath);
        }
        else
        {
            // Download the file from Firebase Storage and save it to local storage
            UnityWebRequest www = UnityWebRequest.Get(storageUrl + "images%2F" + fileName + "?alt=media");
            www.downloadHandler = new DownloadHandlerFile(imagePath + fileName);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                yield break;
            }

            CustomDebug("File " + fileName + " downloaded and saved to " + imagePath);
        }

        // Read the image data from file
        byte[] imageData = File.ReadAllBytes(imagePath + fileName);

        // Create a new texture and load the image data into it
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Create a new sprite from the texture and set it on the Image UI element
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Set the height of the Image UI element
        RectTransform rectTransform = itemImage.GetComponent<RectTransform>();
        float imageRatio = (float)texture.width / texture.height;
        float height = rectTransform.rect.width / imageRatio;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        itemImage.sprite = sprite;
    }

    public void EnterPanel()
    {
        // Enable mouse
        GameState.ShowCursor();
        // Disable camera and player movement
        GameState.DisableCameraMovement();
        GameState.DisablePlayerMovement();
    }

    public void ExitPanel()
    {
        // Disable mouse
        GameState.HideCursor();
        // Enable camera and player movement
        GameState.EnableCameraMovement();
        GameState.EnablePlayerMovement();
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
