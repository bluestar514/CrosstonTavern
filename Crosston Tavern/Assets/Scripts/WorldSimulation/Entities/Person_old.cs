using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person_old
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }
    public string location = "uninitialized";
    public Feature feature; //this is them as a feature, gets moved around the map.

    public List<ExecutedAction> history = new List<ExecutedAction>();

    public Inventory inventory;
    public Relationship relationships;
    public StringStringListDictionary preferences;


    public EmploymentData employmentData;
    public List<FamilyData> family;

    public GoalManager gm;
    public List<Goal> goals;

    public StringStringListDictionary resources;

    public Person_old(string id)
    {
        Id = id;

        resources = new StringStringListDictionary();
        resources.CopyFrom(    new Dictionary<string, List<string>>() {
            {ResourceCatagories.r_initiator, new List<string>(){id } }
        });

        feature = new Feature("person_" + Id, location, 2,
                new List<GenericAction>() { },
                new Dictionary<string, List<string>> { }
            );
        inventory = new Inventory(id);
        relationships = new Relationship();
        family = new List<FamilyData>();

        preferences = new StringStringListDictionary() {
            {"liked", new List<string>(){"red_flower"} },
            {"disliked", new List<string>(){"yellow_flower"} }
        };
    }

    public void Move(string locationId)
    {
        location = locationId;
        feature.location = locationId;
    }


    public string StringifyStats()
    {
        string body = "";

        body += "resources:\n";
        foreach (string key in resources.Keys) {
            body += "\t" + key + ": " + String.Join(", ", resources[key]) + "\n";
        }

        body += "\ninventory:\n";
        body += inventory.Print();

        body += "\nrelationships:\n";
        foreach (string key in relationships.GetKnownPeople()) {
            body += "\t" + key + ": " + relationships.Get(key, Relationship.RelationType.friendly)+ "," + relationships.Get(key, Relationship.RelationType.romantic) + "\n";
        }

        return body;
    }

}


[System.Serializable]
public class EmploymentData
{
    public string title;
    public string establishment;
    public WorldTime shiftStart;
    public WorldTime shiftEnd;
}


[System.Serializable]
public class FamilyData
{
    public string id;
    public string relation;
}