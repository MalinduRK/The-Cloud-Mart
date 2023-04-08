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
    public string fileType = ".glb";
    public string apiKey = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC7KFInf0JYb+/Q";
    public string localPath = "Assets/Models/";

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

        foreach (var item in itemList.items)
        {
            Debug.Log(item.name);
            Debug.Log(item.bucket);

            string[] fileNames = item.name.Split('/');

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith(fileType))
                {
                    Debug.Log("Saving file " + fileName);
                    StartCoroutine(DownloadAndSaveFile(fileName));
                }
            }
        }
    }

    IEnumerator DownloadAndSaveFile(string fileName)
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

        // Load the GLTF file using the GLTFUtility library
        GameObject gltfObject = Importer.LoadFromFile(localPath + fileName);

        // Set the object's name to the file name (without extension)
        gltfObject.name = Path.GetFileNameWithoutExtension(fileName);

        // Position the object in the scene as desired
        gltfObject.transform.position = Vector3.zero;

        // Rotate the object as desired
        gltfObject.transform.rotation = Quaternion.identity;
    }
}
