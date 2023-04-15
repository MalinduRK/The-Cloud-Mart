using UnityEngine;
using Realms;
using Realms.Sync;
using System.Linq;
using static UnityEditor.VersionControl.Message;
using System;

public class MongoController : MonoBehaviour
{
    private App realmApp;
    private Realm realm;
    private IQueryable<ItemProfile> items;

    void Start()
    {
        realmApp = App.Create("your-app-id");

        items = realm.All<ItemProfile>();

        var config = new PartitionSyncConfiguration("my-partition", realmApp.CurrentUser);

        // Insert a new item
        realm.Write(() =>
        {
            var newItem = realm.Add(new ItemProfile
            {
                Name = "New Item",
                Price = 10.99m,
                Description = "A new item"
            });
        });

        // Query items
        var expensiveItems = items.Where(i => i.Price > 50.0m);

        foreach (var item in expensiveItems)
        {
            Debug.Log(item.Name + " - " + item.Price);
        }
    }
}