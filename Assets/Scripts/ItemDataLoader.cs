using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

public class ItemDataLoader : MonoBehaviour
{
    public bool debug;
    public ItemDataStore dataStore;
    // This is a C# event
    public event Action<string[]> OnDocumentLoaded;

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
        string url = $"https://firestore.googleapis.com/v1/projects/the-cloud-mart/databases/(default)/documents/cupboard/";

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
                CustomDebug("No. of documents: " + documents.Count.ToString());

                // Initialize an array of document ids to be sent to the datastore
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
                        Debug.Log("Value of modelAdded variable: " + modelAdded);

                        if (modelAdded == "True")
                        {
                            Debug.Log("Model added");
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
                        }
                    }
                }

                //dataStore.documentId = docIdArray;
                if (docIdArray != null)
                {
                    OnDocumentLoaded(docIdArray);
                }
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
