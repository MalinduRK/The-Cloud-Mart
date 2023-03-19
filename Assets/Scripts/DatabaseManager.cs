using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class DatabaseManager : MonoBehaviour
{
    public TMP_Text itemNameText;

    public TMP_Text item1Title;
    public TMP_Text item2Title;
    public TMP_Text item3Title;

    string itemName;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(retrieveFromDatabase());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateItem()
    {
        itemNameText.text = itemName;
    }

    IEnumerator retrieveFromDatabase()
    {
        // Construct the URL for the Firestore document
        string url = $"https://firestore.googleapis.com/v1/projects/the-cloud-mart/databases/(default)/documents/items/item0001?key=MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC7KFInf0JYb+/Q";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Deserialize the response content to a JObject
                JObject jsonResponse = JObject.Parse(www.downloadHandler.text);

                // Get the value of the nested field
                itemName = jsonResponse["fields"]["itemName"]["stringValue"].ToString();
                UpdateItem();

                Debug.Log(itemName);
            }
        }
    }
}
