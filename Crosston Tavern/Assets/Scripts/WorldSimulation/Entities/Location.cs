using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location 
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }

    public StringStringListDictionary resources;

    public Location(string id, Dictionary<string, List<string>> resources)
    {
        Id = id;
        this.resources = new StringStringListDictionary();
        this.resources.CopyFrom(resources);
    }

    public override string ToString()
    {
        return "-"+id+"-";
    }

    public string ToStringVerbose()
    {
        List<string> resourceList = new List<string>();
        foreach(string key in resources.Keys) {
            resourceList.Add("<"+key + ":" + String.Join(",", resources[key])+">");
        }



        return "-" + id + "{" + String.Join(",", resourceList) + "}-";
    }
}
