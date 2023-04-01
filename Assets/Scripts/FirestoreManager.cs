using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

public class FirestoreManager : MonoBehaviour
{
    async void Start()
    {
        string projectId = "your-project-id";
        string collectionName = "items";
        string url = $"https://firestore.googleapis.com/v1beta1/projects/{projectId}/databases/(default)/documents/{collectionName}?pageSize=4&orderBy=createTime&mask.fieldPaths=modelLoaded&where=modelLoaded=true";

        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        string json = await response.Content.ReadAsStringAsync();

        Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        List<Dictionary<string, object>> documents = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data["documents"].ToString());

        foreach (var document in documents)
        {
            // Extract data from document fields as needed
            Debug.Log(document["name"].ToString());
            Debug.Log(document["createTime"].ToString());
            //Debug.Log(document["fields"]["modelLoaded"]["booleanValue"].ToString());
            Debug.Log(document["fields"].ToString());
        }
    }
}
