using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Siccity.GLTFUtility;
using Newtonsoft.Json;

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
    public string storageUrl = "https://firebasestorage.googleapis.com/v0/b/the-cloud-mart.appspot.com/o/";
    public string bucketPath = "models/";
    public string apiKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC7KFInf0JYb+/Q";
    public string localPath = "Assets/Models/";
    // Create a parent object to hold all the downloaded objects
    public GameObject modelLoaderObject;
    public GameObject platformObject;

    IEnumerator Start()
    {
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

        // Set initial position for the first game object
        Vector3 position = modelLoaderObject.transform.position;

        // Set the scale of the parent object to perform correct positioning of the objects
        float parentX = 3;
        float parentY = 1;
        float parentZ = 3;

        // Counter for placing the objects in different positions
        int counter = 1;

        foreach (var item in itemList.items)
        {
            //Debug.Log(item.name);
            //Debug.Log(item.bucket);

            string[] fileNames = item.name.Split('/');

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(".glb") || fileName.EndsWith(".gltf"))
                {
                    switch(counter)
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
                            break;
                    }
                    StartCoroutine(DownloadAndSaveFile(fileName, position));
                    counter++;
                }
            }
        }
    }

    IEnumerator DownloadAndSaveFile(string fileName, Vector3 position)
    {
        string filePath = localPath + fileName;

        // Check if the file already exists
        if (File.Exists(filePath))
        {
            Debug.Log("File " + fileName + " already exists in " + localPath);
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

            Debug.Log("File " + fileName + " downloaded and saved to " + localPath);
        }

        // Load the GLTF file using the GLTFUtility library
        GameObject gltfObject = Importer.LoadFromFile(localPath + fileName);

        // Set the object's name to the file name (without extension)
        gltfObject.name = Path.GetFileNameWithoutExtension(fileName);

        // Set the parent object
        gltfObject.transform.SetParent(platformObject.transform);

        // Add a BoxCollider to the object
        gltfObject.AddComponent<BoxCollider>();

        // Position the object in the scene as desired
        gltfObject.transform.position = position;

        // Rotate the object as desired
        // gltfObject.transform.rotation = Quaternion.identity;
    }
}
