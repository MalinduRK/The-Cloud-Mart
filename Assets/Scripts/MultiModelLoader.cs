using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

public class MultiModelLoader : MonoBehaviour
{
    GameObject wrapper;
    string filePath;
    float distanceBetweenObjects = 2f; // set the distance between objects here

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/";
        wrapper = new GameObject
        {
            name = "Models"
        };

        DownloadFiles("https://firebasestorage.googleapis.com/v0/b/the-cloud-mart.appspot.com/o/?prefix=models/");
    }

    public void DownloadFiles(string url)
    {
        StartCoroutine(GetFilesRequest(url, (UnityWebRequest req) =>
        {
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Split the response into individual file URLs
                string[] fileUrls = req.downloadHandler.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Log(req.downloadHandler.text);
                foreach (string fileUrl in fileUrls)
                {
                    Debug.Log(fileUrl);
                    if (fileUrl.EndsWith(".glb\","))
                    {
                        // Remove the prefix and suffix from the file URL
                        string fileName = Path.GetFileName(fileUrl);
                        Debug.Log($"Downloading file {fileName}");
                        string downloadUrl = fileUrl + "?alt=media";

                        Debug.Log(downloadUrl);

                        // Download the file and create a new object
                        DownloadFile(downloadUrl, fileName);
                    }
                }
            }
        }));
    }

    public void DownloadFile(string url, string fileName)
    {
        string path = $"{filePath}{fileName}";
        if (File.Exists(path))
        {
            Debug.Log($"Found {fileName} locally, loading...");
            CreateModel(path);
            return;
        }

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        {
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Save the model into a new wrapper
                CreateModel(path);
            }
        }));
    }

    void CreateModel(string path)
    {
        // Import the GLB file and create a new object
        GameObject model = Importer.LoadFromFile(path);
        model.name = Path.GetFileNameWithoutExtension(path);

        // Set the position of the new object based on the number of objects already created
        int numObjects = wrapper.transform.childCount;
        Vector3 position = new Vector3(numObjects * distanceBetweenObjects, 0f, 0f);
        model.transform.position = position;

        // Set the parent of the new object to the wrapper object
        model.transform.SetParent(wrapper.transform);
    }

    IEnumerator GetFilesRequest(string url, Action<UnityWebRequest> callback)
    {
        using UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
        callback(req);
    }

    IEnumerator GetFileRequest(string url, Action<UnityWebRequest> callback)
    {
        using UnityWebRequest req = UnityWebRequest.Get(url);
        req.downloadHandler = new DownloadHandlerFile(GetFilePath(url));
        yield return req.SendWebRequest();
        callback(req);
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[^1];
        return $"{Application.persistentDataPath}/Files/{filename}";
    }
}
