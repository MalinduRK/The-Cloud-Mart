using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Siccity.GLTFUtility;

public class ModelLoader : MonoBehaviour
{
    GameObject wrapper;
    string filePath;

    private void Start()
    {
        filePath = $"{Application.persistentDataPath}/Files/";
        wrapper = new GameObject
        {
            name = "Model"
        };

        // %2F is used in between the folder name and filename in firebase request urls, instead of a /

        DownloadFile("https://firebasestorage.googleapis.com/v0/b/the-cloud-mart.appspot.com/o/armchair.glb");
    }
    public void DownloadFile(string url)
    {
        string url2 = url + "?alt=media";
        string path = GetFilePath(url);
        if (File.Exists(path))
        {
            Debug.Log("Found file locally, loading...");
            LoadModel(path);
            return;
        }

        StartCoroutine(GetFileRequest(url, url2, (UnityWebRequest req) =>
        {
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log any errors that may happen
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            }
            else
            {
                // Save the model into a new wrapper
                LoadModel(path);
            }
        }));
    }

    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        string filename = pieces[^1];
        Debug.Log(filename);

        return $"{filePath}{filename}";
    }

    void LoadModel(string path)
    {
        ResetWrapper();
        GameObject model = Importer.LoadFromFile(path);
        model.transform.SetParent(wrapper.transform);
    }

    IEnumerator GetFileRequest(string pathUrl, string downloadUrl, Action<UnityWebRequest> callback)
    {
        using UnityWebRequest req = UnityWebRequest.Get(downloadUrl);
        req.downloadHandler = new DownloadHandlerFile(GetFilePath(pathUrl));
        yield return req.SendWebRequest();
        callback(req);
    }

    void ResetWrapper()
    {
        if (wrapper != null)
        {
            foreach (Transform trans in wrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }
    }
}