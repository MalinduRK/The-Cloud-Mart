using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Siccity.GLTFUtility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private bool firestoreDataLoaded = false;

    IEnumerator Start()
    {
        ItemDataLoader firestoreReader = FindObjectOfType<ItemDataLoader>();
        // Subscribe to the loader event from ItemDataLoader
        if (firestoreReader != null)
        {
            firestoreReader.OnDocumentLoaded += DocumentLoadedCallback;
        }

        localPath = $"{Application.persistentDataPath}/Files/Models/";
        // Define array to store names of all files in the firebase storage
        string[] itemArray = new string[0];

        // Load all files from Firebase Storage with .glb extension
        UnityWebRequest www = UnityWebRequest.Get(storageUrl + "?prefix=" + bucketPath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            yield break;
        }

        // Parse response and download each file to local storage
        string response = www.downloadHandler.text;

        // Using newtonsoft json parser, we can easily read the json data
        ItemList itemList = JsonConvert.DeserializeObject<ItemList>(response);

        // Counter for items
        int itemCounter = 0;
        // Array size changer
        int arraySize = 0;

        foreach (var item in itemList.items)
        {
            //Debug.Log(item.name);
            //Debug.Log(item.bucket);

            string[] fileNames = item.name.Split('/');

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(".glb") || fileName.EndsWith(".gltf"))
                {
                    // Add 1 to array size whenever a new item is introduced
                    arraySize++;
                    Array.Resize(ref itemArray, arraySize);
                    // Add item to the array one by one for later use
                    itemArray[itemCounter] = fileName;
                    //Debug.Log("item array = " + itemArray[itemCounter]);
                    itemCounter++;
                }
            }
        }
        //Debug.Log("Array size: " + itemArray.Length);
        /*for (int i = 0; i < itemArray.Length; i++)
        {
            Debug.Log(itemArray[i]);
        }*/
        //PaginateItems(itemArray);
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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.gameObject == buttonObject)
        {
            promptText.text = "Load more items\n(LMB)\n(" + pageNumber + "/" + totalPages + ")";
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                CustomDebug("Mouse button pressed");
                StartCoroutine(LoadFiles());
            }
        }
        else
        {
            promptText.text = "";
        }
    }

    private void DocumentLoadedCallback(string[] documentData)
    {
        // Set the flag to indicate that the Firestore data has been loaded
        firestoreDataLoaded = true;

        PaginateItems(documentData);
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
                    Debug.Log("Name of item " + (j+itemCount) + itemName[j + itemCount]);
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

    IEnumerator DownloadAndSaveFile(string fileName, Vector3 position)
    {
        fileName += "_model.glb";
        Debug.Log(fileName);
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
    }

    private void CustomDebug(string message)
    {
        if (debug)
        {
            Debug.Log(message);
        }
    }
}
