using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureInitializer 
{
    static string UNSET = "UNSET";



    static Dictionary<Feature.FeatureType, Feature> MakeFeatures()
    {
        return new Dictionary<Feature.FeatureType, Feature>() {
            {Feature.FeatureType.river, new Feature("river", Feature.FeatureType.river, UNSET, 2,
                                    new List<GenericAction>(){
                                        ActionInitializer.actions["fish"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"common_fish", new List<string>(){"trout", "bass"} },
                                        {"rare_fish", new List<string>(){"salmon"} }
                                    })
            },
            {Feature.FeatureType.door, new Feature("door", Feature.FeatureType.door, UNSET, 100,
                                    new List<GenericAction>(){ ActionInitializer.actions["move"] },
                                    new Dictionary<string, List<string>>())
            },
            {Feature.FeatureType.shop, new Feature("shop", Feature.FeatureType.shop, UNSET, 2,
                                    new List<GenericAction>(){ ActionInitializer.actions["buy_#item#"],
                                                                ActionInitializer.actions["outing_shopping_at_#loc#_with_#b#"]
                                    },
                                    new Dictionary<string, List<string>>(),
                                    new Dictionary<string, int>(){})
            }
        };
    }

    static Dictionary<string, List<Feature.FeatureType>> featuresInRoom = new Dictionary<string, List<Feature.FeatureType>>() {
        {"farm", new List<Feature.FeatureType>(){ Feature.FeatureType.river} },
        {"feild", new List<Feature.FeatureType>(){ Feature.FeatureType.river} }
    };

    static List<KeyValuePair<string, string>> roomConnections = new List<KeyValuePair<string, string>>() {
        new KeyValuePair<string, string>("farm", "feild"),
        new KeyValuePair<string, string>("feild", "hill"),
        new KeyValuePair<string, string>("feild", "forest"),
        new KeyValuePair<string, string>("feild", "town"),
        new KeyValuePair<string, string>("town", "inn"),
        new KeyValuePair<string, string>("town", "blacksmith")
    };


    public static Dictionary<string, Feature> GetAllFeatures()
    {
        Dictionary<string, Feature> allFeatures = new Dictionary<string, Feature>();

        foreach(string room in featuresInRoom.Keys) {
            foreach(Feature.FeatureType ft in featuresInRoom[room]) {
                Feature newFeature = MakeFeatures()[ft];
                newFeature.location = room;
                newFeature.id += "_" + newFeature.location;

                allFeatures.Add(newFeature.id, newFeature);
            }
        }


        Feature shop = MakeFeatures()[Feature.FeatureType.shop];
        shop.id = "tackle_shop_town";
        shop.location = "farm";
        shop.stockTable = new StringIntDictionary() {
            {"fishing_rod", 5 },
            {"bass", 2},
            {"trout", 2},
            {"salmon", 4}
        };
        shop.inventory.ChangeInventoryContents(10, "fishing_rod");
        shop.inventory.ChangeInventoryContents(20, "bass");

        allFeatures.Add("tackle_shop_town", shop);


        foreach (KeyValuePair<string, string> connection in roomConnections) {
            Feature door = MakeFeatures()[Feature.FeatureType.door];
            door.location = connection.Key;
            door.relevantResources.Add(Map.R_CONNECTEDLOCATION, new List<string>() { connection.Value });
            door.id += "_" + door.location + "->" + connection.Value;

            allFeatures.Add(door.id, door);

            door = MakeFeatures()[Feature.FeatureType.door];
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
            {"feild", new Location("feild", new Dictionary<string, List<string>>(){ }) },
            {"hill", new Location("hill", new Dictionary<string, List<string>>(){ }) },
            {"inn", new Location("inn", new Dictionary<string, List<string>>(){ }) },
            {"forest", new Location("forest", new Dictionary<string, List<string>>(){ }) },
            {"town", new Location("town", new Dictionary<string, List<string>>(){ }) },
            {"blacksmith", new Location("blacksmith", new Dictionary<string, List<string>>(){ }) }
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
