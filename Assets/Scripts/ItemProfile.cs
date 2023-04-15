using MongoDB.Bson;
using Realms;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProfile : RealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId Id { get; set; }

    [MapTo("itemName")]
    public string Name { get; set; }

    [MapTo("itemPrice")]
    public decimal Price { get; set; }

    [MapTo("itemDescription")]
    public string Description { get; set; }
}
