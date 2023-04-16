using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class ItemDataLoader : MonoBehaviour
{
    public bool debug;

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
        string url = $"https://firestore.googleapis.com/v1/projects/the-cloud-mart/databases/(default)/documents/items/";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                CustomDebug(www.error);
            }
            else
            {
                /*
                // Deserialize the response content to a JObject
                JObject jsonResponse = JObject.Parse(www.downloadHandler.text);

                // Get the value of the nested field
                var itemName = jsonResponse["documents"]["fields"]["itemName"]["stringValue"].ToString();

                Debug.Log(itemName);
                */

                // Parse the response JSON and put each document into a separate variable
                string responseJson = www.downloadHandler.text;
                JObject response = JObject.Parse(responseJson);
                JArray documents = (JArray)response["documents"];
                foreach (JToken document in documents)
                {
                    // Do something with the document data
                    string documentId = document["name"].ToString();
                    JObject fields = (JObject)document["fields"];
                    // Extract field values from the "fields" JObject
                    // For example, to get the "itemName" field:
                    JToken itemNameToken = fields["itemName"];
                    if (itemNameToken != null)
                    {
                        string itemName = itemNameToken["stringValue"].ToString();
                        CustomDebug("Item name: " + itemName);
                    }
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
