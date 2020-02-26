using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class LoadWorldData : WorldInitializer
{
    JSON totalJsonLocation;
    JSON totalJsonFeature;

    public LoadWorldData(TextAsset locationsJsonFile, TextAsset featuresJsonFile)
    {

        totalJsonLocation = JSON.ParseString(locationsJsonFile.text);
        totalJsonLocation.DebugInEditor("Locations");

        totalJsonFeature = JSON.ParseString(featuresJsonFile.text);
        totalJsonFeature.DebugInEditor("Features");
    }

    protected override List<Location> InitializeLocations()
    {
        Dictionary<string, HashSet<string>> connections = new Dictionary<string, HashSet<string>>();


        foreach (JArray connection in totalJsonLocation.GetJArray("connections").AsJArrayArray()) {
            string[] con = connection.AsStringArray();
            string a = con[0];
            string b = con[1];

            if (connections.ContainsKey(a)) {
                connections[a].Add(b);
            } else {
                connections.Add(a, new HashSet<string>() { b });
            }

            if (connections.ContainsKey(b)) {
                connections[b].Add(a);
            } else {
                connections.Add(b, new HashSet<string>() { a });
            }
        }

        foreach(string key in connections.Keys) {
            Debug.Log(key + ": " + string.Join(",", connections[key]));
        }

        List<Location> locations = new List<Location>();
        foreach(JSON location in totalJsonLocation.GetJArray("locations").AsJSONArray()) {
            string id = location.GetString("id");

            JSON jsonResources = location.GetJSON("resources");
            StringStringListDictionary resources = ParseResources(jsonResources);

            resources.Add(ResourceCatagories.r_connectedLocation, new List<string>(connections[id]));

            locations.Add(new Location(id, resources));
        }


        return locations;
    }

    StringStringListDictionary ParseResources(JSON jsonResources)
    {
        StringStringListDictionary resources = new StringStringListDictionary();
        
        foreach (string jsonResource in jsonResources.Keys) {
            List<string> list = new List<string>(jsonResources.GetJArray(jsonResource).AsStringArray());
            resources.Add(jsonResource, list);
        }

        return resources;
    }

    protected override List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, List<Location> locations, List<Person> people)
    {
        List<Feature> features = new List<Feature>();

        foreach(JSON feature in totalJsonFeature.GetJArray("features").AsJSONArray()) {
            string id = feature.GetString("id");
            string location = feature.GetString("location");

            List<GenericAction> genericActions = new List<GenericAction>();
            foreach (string actionId in feature.GetJArray("actions").AsStringArray()) {
                Debug.Log(actionId);
                genericActions.Add(actions[actionId]);
            }

            JSON jsonResources = feature.GetJSON("resources");
            StringStringListDictionary resources = ParseResources(jsonResources);

            features.Add(new Feature(id, location, genericActions, resources));
        }


        AddDoors(features, actions, locations);

        AddPeople(features, people, actions);

        return features;
    }


}
