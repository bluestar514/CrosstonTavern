using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureInitializer 
{
    static string UNSET = "UNSET";

    enum FeatureType
    {
        river,
        door
    }

    static Dictionary<FeatureType, Feature> MakeFeatures()
    {
        return new Dictionary<FeatureType, Feature>() {
            {FeatureType.river, new Feature("river", UNSET, 2,
                                    new List<GenericAction>(){
                                        ActionInitializer.actions["fish"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"common_fish", new List<string>(){"trout", "bass"} },
                                        {"rare_fish", new List<string>(){"salmon"} }
                                    })
            },
            {FeatureType.door, new Feature("door", UNSET, 100,
                                    new List<GenericAction>(){ ActionInitializer.actions["move"] },
                                    new Dictionary<string, List<string>>())
            }
        };
    }

    static Dictionary<string, List<FeatureType>> featuresInRoom = new Dictionary<string, List<FeatureType>>() {
        {"farm", new List<FeatureType>(){FeatureType.river} },
        {"feild", new List<FeatureType>(){FeatureType.river} }
    };

    static List<KeyValuePair<string, string>> roomConnections = new List<KeyValuePair<string, string>>() {
        new KeyValuePair<string, string>("farm", "feild")
    };


    public static Dictionary<string, Feature> GetAllFeatures()
    {
        Dictionary<string, Feature> allFeatures = new Dictionary<string, Feature>();

        foreach(string room in featuresInRoom.Keys) {
            foreach(FeatureType ft in featuresInRoom[room]) {
                Feature newFeature = MakeFeatures()[ft];
                newFeature.location = room;
                newFeature.id += "_" + newFeature.location;

                allFeatures.Add(newFeature.id, newFeature);
            }
        }

        foreach(KeyValuePair<string, string> connection in roomConnections) {
            Feature door = MakeFeatures()[FeatureType.door];
            door.location = connection.Key;
            door.relevantResources.Add(Map.R_CONNECTEDLOCATION, new List<string>() { connection.Value });
            door.id += "_" + door.location + "->" + connection.Value;

            allFeatures.Add(door.id, door);

            door = MakeFeatures()[FeatureType.door];
            door.location = connection.Value;
            door.relevantResources.Add(Map.R_CONNECTEDLOCATION, new List<string>() { connection.Key });
            door.id += "_" + door.location + "->" + connection.Key;

            allFeatures.Add(door.id, door);
        }

        foreach(KeyValuePair<string, Person> person in PeopleInitializer.GetAllPeople()) {
            allFeatures.Add(person.Key, person.Value);
        }


        return allFeatures;
    }

    public static Dictionary<string, Location> GetAllLocations()
    {
        Dictionary<string, Location> allLocations = new Dictionary<string, Location>() {
            {"farm", new Location("farm", new Dictionary<string, List<string>>(){}) },
            {"feild", new Location("feild", new Dictionary<string, List<string>>(){ }) }
        };

        foreach (KeyValuePair<string, string> connection in roomConnections) {
            Dictionary<string, List<string>> currentResources = allLocations[connection.Key].resources;
            if (!currentResources.ContainsKey(Map.R_CONNECTEDLOCATION)) currentResources.Add(Map.R_CONNECTEDLOCATION, new List<string>());
            currentResources[Map.R_CONNECTEDLOCATION].Add(connection.Value);


            currentResources = allLocations[connection.Value].resources;
            if (!currentResources.ContainsKey(Map.R_CONNECTEDLOCATION)) currentResources.Add(Map.R_CONNECTEDLOCATION, new List<string>());
            currentResources[Map.R_CONNECTEDLOCATION].Add(connection.Key);
        }

        return allLocations;
    }
}
