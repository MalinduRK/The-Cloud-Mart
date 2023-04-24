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
    public bool debug;
    public string storageUrl = "https://firebasestorage.googleapis.com/v0/b/the-cloud-mart.appspot.com/o/";
    public string bucketPath = "models/";
    public string apiKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC7KFInf0JYb+/Q";
    public string localPath;
    // Create a parent object to hold all the downloaded objects
    public GameObject modelLoaderObject;
    public GameObject parentObject;
    // Create a dictionary to store the items in separate, dynamically created arrays
    Dictionary<string, string[]> pages = new Dictionary<string, string[]>();
    // Keep track of page number in display
    private int pageNumber = 0;
    private int totalPages = 0;
    // Loaded models
    private GameObject gltfObject;
    //
    // Variables related to loading more items
    public Text promptText;
    public Camera mainCamera;
    // Distance where the text prompt is triggered
    public float maxDistance = 2.5f;
    public GameObject buttonObject;

    // Firestore data
    public ItemDataStore dataStore;
    // Dictionary for storing all firestore data with the respective item ID
    Dictionary<string, ItemDataStore> items = new Dictionary<string, ItemDataStore>();
    private bool firestoreDataLoaded = false;

    // Item displays
    private GameObject textObjectPrefab;
    private TextMeshPro textMeshPro;

    void Start()
    {
        ItemDataLoader firestoreReader = FindObjectOfType<ItemDataLoader>();
        // Subscribe to the loader event from ItemDataLoader
        if (firestoreReader != null)
        {
            firestoreReader.OnDocumentLoaded += DocumentLoadedCallback;
        }

        localPath = $"{Application.persistentDataPath}/Files/Models/";

        // Create the child object prefab with the TextMeshPro component
        textObjectPrefab = new GameObject("TextMeshPro");
        TextMeshPro textMeshPro = textObjectPrefab.AddComponent<TextMeshPro>();

        // Set the textMeshPro properties as desired
        textMeshPro.text = "Your Text Here";
        textMeshPro.fontSize = 24f;
        textMeshPro.color = Color.white;
    }

    private void Update()
    {
        ItemDataLoader firestoreReader = FindObjectOfType<ItemDataLoader>();
        // Check if the firestore data had loaded up and is ready
        if (firestoreDataLoaded && dataStore != null)
        {
            firestoreReader.OnDocumentLoaded += DocumentLoadedCallback;
        }

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Only work if the app isn't paused
        if (!PauseMenu.isPaused)
        {
            // Check if the player camera is looking at the said object
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance) && hit.collider.gameObject == buttonObject)
            {
                promptText.text = "Load more items\n[Left Mouse Button]\n(" + pageNumber + "/" + totalPages + ")";
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    CustomDebug("Mouse button pressed");
                    StartCoroutine(LoadFiles());
                }
            }
            // Check if the player is looking at a loaded item and take action
            else if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.transform.parent != null && hit.collider.transform.parent.gameObject.name == "Items")
            {
                promptText.text = "View details\n[Left Mouse Button]";
            }
            else
            {
                promptText.text = "";
            }
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

                    string imageFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["imageFileExtension"]["stringValue"] != null)
                    {
                        imageFileExtension = fields["imageFileExtension"]["stringValue"].ToString();
                    }

                    string modelFileExtension = "";
                    // Add extension value only if it is not null
                    if (fields["imageFileExtension"]["stringValue"] != null)
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
        Vector3 position = modelLoaderObject.transform.position;

        // Set the scale of the parent object to perform correct positioning of the objects
        float parentX = 3;
        float parentY = 1;
        float parentZ = 3;

        for (int i = 0; i < pages[page].Length; i++)
        {
            //Debug.Log("Page length: " + pages[page].Length);
            // Counter to get the positions of loaded items
            int counter = i + 1;

            switch (counter)
            {
                case 1:
                    position += new Vector3(0 * parentX, 0 * parentY, 0 * parentZ);
                    break;
                case 2:
                    position += new Vector3(0 * parentX, 0 * parentY, (float)(1.5 * parentZ));
                    break;
                case 3:
                    position += new Vector3((float)(1.5 * parentX), 0 * parentY, 0 * parentZ);
                    break;
                case 4:
                    position += new Vector3(0 * parentX, 0 * parentY, (float)(-1.5 * parentZ));
                    break;
                default:
                    Debug.Log("Wrong counter in switch statement!");
                    yield break;
            }

            StartCoroutine(DownloadAndSaveFile(pages[page][i], position));
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
        string filePath = localPath + fileName;

        // Check if the file already exists
        if (File.Exists(filePath))
        {
            CustomDebug("File " + fileName + " already exists in " + localPath);
        }
        else
        {
            // Download the file from Firebase Storage and save it to local storage
            UnityWebRequest www = UnityWebRequest.Get(storageUrl + "models%2F" + fileName + "?alt=media");
            www.downloadHandler = new DownloadHandlerFile(localPath + fileName);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                yield break;
            }

            CustomDebug("File " + fileName + " downloaded and saved to " + localPath);
        }

        // Load the GLTF file using the GLTFUtility library
        gltfObject = Importer.LoadFromFile(localPath + fileName);

        // Set the object's name to the file name (without extension)
        gltfObject.name = Path.GetFileNameWithoutExtension(fileName);

        // Set the parent object
        gltfObject.transform.SetParent(parentObject.transform);

        // Calculate the bounding box of the model
        Bounds bounds = gltfObject.GetComponentInChildren<MeshRenderer>().bounds;

        // Calculate the scale factor needed to fit the bounding box within a desired size
        float desiredSize = 2f; // desired size in world units
        float scaleFactor = desiredSize / bounds.size.magnitude;

        // Apply the scale factor to the model
        gltfObject.transform.localScale *= scaleFactor;

        // Add a BoxCollider to the object
        gltfObject.AddComponent<BoxCollider>();

        // Position the object in the scene as desired
        gltfObject.transform.position = position;

        // Rotate the object as desired
        // gltfObject.transform.rotation = Quaternion.identity;

        GameObject textObject = GameObject.Instantiate(textObjectPrefab);
        textObject.transform.SetParent(gltfObject.transform);

        /*
        // Attach a TextMeshPro component to the game object
        TextMeshPro textMeshPro = gltfObject.AddComponent<TextMeshPro>();

        // Set the text of the TextMeshPro component
        textMeshPro.text = $"Item Name: {items[fileId].ItemName}\nItem Description: {items[fileId].ItemDescription}\nItem Price: {items[fileId].ItemPrice}";
        */
    }

    private void CustomDebug(string message)
    {
        if (debug)
        {
            Debug.Log(message);
        }
    }
}
