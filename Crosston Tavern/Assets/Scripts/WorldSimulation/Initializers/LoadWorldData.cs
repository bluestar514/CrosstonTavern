using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadWorldData : WorldInitializer
{
    JSONLocationFile jsonLocObj;
    JSONFeatureFile jsonFeatObj;

    public LoadWorldData(TextAsset locationsJsonFile, TextAsset featuresJsonFile)
    {
        jsonLocObj = JsonUtility.FromJson<JSONLocationFile>(locationsJsonFile.text);

        jsonFeatObj = JsonUtility.FromJson<JSONFeatureFile>(featuresJsonFile.text);
    }

    protected override List<Location> InitializeLocations()
    {
        Dictionary<string, HashSet<string>> connections = new Dictionary<string, HashSet<string>>();


        foreach(JSONPairs pair in jsonLocObj.connections) {
            List<string> connection = pair.a;
            string a = connection[0];
            string b = connection[1];

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
        foreach(JSONLocation jsonLoc in jsonLocObj.locations) {
            Dictionary<string, List<string>> resources = jsonLoc.resources;
            resources.Add(ResourceCatagories.r_connectedLocation,new List<string>(connections[jsonLoc.id]));
            locations.Add(new Location(jsonLoc.id, resources));
        }

        return locations;
    }

    protected override List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, List<Location> locations, List<Person> people)
    {
        List<Feature> features = new List<Feature>();

        foreach(JSONFeature jsonFeat in jsonFeatObj.features) {
            List<GenericAction> genericActions = new List<GenericAction>();
            foreach(string actionId in jsonFeat.actions) {
                Debug.Log(actionId);
                genericActions.Add(actions[actionId]);
            }

            features.Add(new Feature(jsonFeat.id, jsonFeat.location, genericActions, jsonFeat.resources));
        }


        AddDoors(features, actions, locations);

        AddPeople(features, people, actions);

        return features;
    }

    [System.Serializable]
    class JSONPairs
    {
        public List<string> a;
    }

    [System.Serializable]
    class JSONLocationFile
    {
        public List<JSONLocation> locations;
        public List<JSONPairs> connections;
    }

    [System.Serializable]
    class JSONLocation
    {
        public string id;
        public StringStringListDictionary resources;
    }

    [System.Serializable]
    class JSONFeatureFile
    {
        public List<JSONFeature> features;
    }

    [System.Serializable]
    class JSONFeature
    {
        public string id;
        public string location;
        public List<string> actions;
        public StringStringListDictionary resources;
    }
}
