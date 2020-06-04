using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
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

    public void Add(string key, string value)
    {
        if (!resources.ContainsKey(key)) {
            resources.Add(key, new List<string>());
        }

        if (!resources[key].Contains(value)) {
            resources[key].Add(value);
        }
    }

    public List<string> BindString(string str)
    {
        if (resources == null) return new List<string>() { str };

        foreach (string binding in resources.Keys) {
            if(str == "#" + binding + "#") return  resources[binding];
        }

        return new List<string>() { str };
    }

    public string PickBoundString(string str)
    {
        List<string> options = BindString(str);

        return options[Random.Range(0, options.Count)];
    }

    public override string ToString()
    {
        return string.Join(",", from resource in resources
                                select resource.Key + ":"+ resource.Value);
    }
}
