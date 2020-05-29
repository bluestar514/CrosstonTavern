using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Map 
{
    [SerializeField]
    protected string id;

    [SerializeField]
    List<Feature> Features;
    [SerializeField]
    List<Location> Locations;

    Dictionary<string, Feature> features;
    Dictionary<string, Location> locations;

    public int LocationCount => Locations.Count;
    public int FeatureCount => Features.Count;

    public static string R_CONNECTEDLOCATION = ResourceCatagories.r_connectedLocation;

    public Map(List<Feature> features, List<Location> locations, string idTag = "default")
    {
        Features = features;
        Locations = locations;

        this.features = new Dictionary<string, Feature>();
        foreach(Feature feature in Features) {
            this.features.Add(feature.id, feature);
        }

        this.locations = new Dictionary<string, Location>();
        foreach(Location location in Locations) {
            this.locations.Add(location.Id, location);
        }

        id = idTag;
    }

    public Map Copy(string name="copy")
    {
        List<Feature> copiedFeatures = new List<Feature>();
        foreach(Feature f in features.Values) {
            copiedFeatures.Add(f.Copy(false));
        }

        return new Map(copiedFeatures, new List<Location>(locations.Values), name);
    }

    public Feature GetFeature(string id)
    {
        if (features.ContainsKey(id)) return features[id];
        return null;
    }
    public Location GetLocation(string id)
    {
        if (locations.ContainsKey(id)) return locations[id];
        return null;
    }

    public bool Neighboring(string source, string dest)
    {
        Location sLoc = GetLocation(source);

        if (sLoc == null) throw new Exception("Location " + source + " does not exist");

        return sLoc.resources[R_CONNECTEDLOCATION].Contains(dest);
    }

    public int GetDistance(string source, string dest)
    {
        List<List<string>> queue = new List<List<string>>() { new List<string>() { source } };
        HashSet<string> discovered = new HashSet<string>() { source };

        int layerNum = 0;

        while(queue.Count > 0) {
            List<string> layer = queue[0];
            List<string> nextLayer = new List<string>();
            queue.RemoveAt(0);

            while (layer.Count > 0) {
                string next = layer[0];
                layer.RemoveAt(0);
                if (next == dest) return layerNum;

                if (!locations.ContainsKey(next)) throw new Exception("location " + next + " not found");

                foreach (string location in locations[next].resources[R_CONNECTEDLOCATION]) {
                    if (!discovered.Contains(location)) {
                        discovered.Add(location);
                        nextLayer.Add(location);
                    }
                }

            }
            queue.Add(nextLayer);
            layerNum++;
        }

        return -1;

    }

    public void MovePerson(string person, string locationId, bool respectDoors = true)
    {
        if (GetFeature(person) == null) throw new Exception("Person " + person + " does not exist");
        if (GetLocation(locationId) == null) throw new Exception("Location " + locationId + " does not exist");

        if (!respectDoors || Neighboring(GetFeature(person).location, locationId)) {
            GetFeature(person).location = locationId;
        }

    }

    public List<Feature> GatherFeaturesAt(string locationId)
    {
        List<Feature> nearbyFeatures = new List<Feature>(from feature in features
                                                         where feature.Value.location == locationId
                                                         select feature.Value);
        return nearbyFeatures;
    }

    public List<string> GetNameOfLocations()
    {
        return new List<string>(from location in Locations
                            select location.Id);
    }

    public List<Feature> GetAllFeatures()
    {
        return new List<Feature>(Features);
    }

    public void RemoveFeature(string featureName)
    {
        Feature f = features[featureName];
        features.Remove(featureName);
        Features.Remove(f);
    }
    public void AddFeature(string key, Feature f)
    {
        features.Add(key, f);
        Features.Add(f);
    }
}
