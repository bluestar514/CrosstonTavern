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

    }

    

}
