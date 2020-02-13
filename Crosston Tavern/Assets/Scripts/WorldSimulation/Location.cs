using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location 
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }

    public Dictionary<string, List<string>> resources;

    public Location(string id, Dictionary<string, List<string>> resources)
    {
        Id = id;
        this.resources = resources;
    }
}
