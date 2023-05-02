using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;
using System;

public class ItemDataLoader : MonoBehaviour
{
    public bool debug;
    public ItemDataStore dataStore;
    // This is a C# event
    // public event Action<JArray> OnDocumentLoaded;
    // An array to loop through all categories in the firestore database
    private string[] categories = { "chair", "bed", "table", "other" };

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
        // Loop through the array of categories and save a separate json file for each category
        for (int i = 0; i < categories.Length; i++)
        {
            string category = categories[i];

            // Construct the URL for the Firestore document
            string url = $"https://firestore.googleapis.com/v1/projects/the-cloud-mart/databases/(default)/documents/" + category + "/";
            string folderPath = $"{Application.persistentDataPath}/Files/json/{category}";
            string fileName = $"{category}.json";

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

                    // Create the directory if it does not exist
                    if (!System.IO.Directory.Exists(folderPath))
                    {
                        System.IO.Directory.CreateDirectory(folderPath);
                    }

                    string filePath = System.IO.Path.Combine(folderPath, fileName); // Combine the folder path and file name to get the full file path

                    JObject response = JObject.Parse(responseJson);
                    JArray documents = (JArray)response["documents"];
                    //CustomDebug(documents.ToString());

                    if (documents != null)
                    {
                        //OnDocumentLoaded(documents);
                        // Save the response JSON to a file
                        System.IO.File.WriteAllText(filePath, responseJson);
                    }

                    //CustomDebug("No. of documents: " + documents.Count.ToString());
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
