using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }
    public string location = "uninitialized";
    public Feature feature; //this is them as a feature, gets moved around the map.

    public List<ExecutedAction> history = new List<ExecutedAction>();

    public StringIntDictionary inventory;
    public StringFloatDictionary relationships;
    public string placeOfWork="unemployed"; //a feautreID

    public Dictionary<MicroEffect, float> goalPriorityDict;

    public StringStringListDictionary resources;

    public Person(string id)
    {
        Id = id;

        resources = new StringStringListDictionary();
        resources.CopyFrom(    new Dictionary<string, List<string>>() {
            {ResourceCatagories.r_initiator, new List<string>(){id } }
        });

        feature = new Feature("person_" + Id, location, 2,
                new List<GenericAction>() { },
                new Dictionary<string, List<string>> {
                    {ResourceCatagories.r_recipient, new List<string>() { id } }
                }
            );
        inventory = new StringIntDictionary();
        relationships = new StringFloatDictionary();
    }

    public void Move(string locationId)
    {
        location = locationId;
        feature.location = locationId;
    }

    public int GetInventoryCount(string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            return inventory[itemId];
        } else {
            return 0;
        }
    }
    public void ChangeInventoryContents(int num, string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            inventory[itemId] += num;
        } else {
            inventory.Add(itemId, num);
        }
    }

    public float GetRelationshipValue(string personId)
    {
        if (relationships.ContainsKey(personId)) {
            return relationships[personId];
        } else {
            return 0;
        }
    }

    public void ChangeRelationshipValue(float num, string personId)
    {
        if (relationships.ContainsKey(personId)) {
            relationships[personId] += num;
        } else {
            relationships.Add(personId, num);
        }
    }

    public string StringifyStats()
    {
        string body = "";

        body += "resources:\n";
        foreach (string key in resources.Keys) {
            body += "\t" + key + ": " + String.Join(", ", resources[key]) + "\n";
        }

        body += "\ninventory:\n";
        foreach(string key in inventory.Keys) {
            body += "\t" + key + ": " + inventory[key] + "\n";
        }

        body += "\nrelationships:\n";
        foreach (string key in relationships.Keys) {
            body += "\t" + key + ": " + relationships[key] + "\n";
        }

        return body;
    }

}
