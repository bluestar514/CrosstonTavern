using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureResources 
{
    public StringStringListDictionary resources;

    public FeatureResources(StringStringListDictionary resources)
    {
        this.resources = resources;
    }

    public void Add(string key, List<string> value)
    {
        resources.Add(key, value);
    }

    public List<string> BindString(string str)
    {
        if (resources == null) return new List<string>() { str };

        foreach (string binding in resources.Keys) {
            if(str == "#" + binding + "#") return  resources[binding];
        }

        return new List<string>() { str };
    }

    public override string ToString()
    {
        return string.Join(",", from resource in resources
                                select resource.Key + ":"+ resource.Value);
    }
}
