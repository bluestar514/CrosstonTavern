﻿using System;
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

    public Dictionary<string, int> inventory;
    public Dictionary<string, float> relationships;

    public Dictionary<MicroEffect, float> goalPriorityDict;

    public Dictionary<string, List<string>> resources;

    public Person(string id)
    {
        Id = id;

        resources = new Dictionary<string, List<string>>() {
            {ResourceCatagories.r_initiator, new List<string>(){id } }
        };

        feature = new Feature("person_" + Id, location,
                new List<GenericAction>() { },
                new Dictionary<string, List<string>> {
                    {ResourceCatagories.r_recipient, new List<string>() { id } }
                }
            );
        inventory = new Dictionary<string, int>();
        relationships = new Dictionary<string, float>();
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
