using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;
using System;

public class ItemDataLoader : MonoBehaviour
{
    public bool debug;
    public ItemDataStore dataStore;
    public string collectionName;
    // This is a C# event
    public event Action<JArray> OnDocumentLoaded;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(retrieveFromDatabase());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator retrieveFromDatabase()
    {
        // Construct the URL for the Firestore document
        string url = $"https://firestore.googleapis.com/v1/projects/the-cloud-mart/databases/(default)/documents/" + collectionName + "/";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                CustomDebug(www.error);
            }
            else
            {
                Debug.Log("Retrieving from firestore");
                // Parse the response JSON and put each document into a separate variable
                string responseJson = www.downloadHandler.text;
                JObject response = JObject.Parse(responseJson);
                JArray documents = (JArray)response["documents"];
                //CustomDebug(documents.ToString());

                if (documents != null)
                {
                    OnDocumentLoaded(documents);
                }

                //CustomDebug("No. of documents: " + documents.Count.ToString());
            }
        }
    }

    private void CustomDebug(string message)
    {
        if (debug)
        {
            Debug.Log(message);
        }
    }
}
