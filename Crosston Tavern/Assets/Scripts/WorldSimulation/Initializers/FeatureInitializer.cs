using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FeatureInitializer 
{
    static string UNSET = "UNSET";



    static Dictionary<string, Feature> MakeFeatures()
    {
        Dictionary<string, GenericAction> actions = ActionInitializer.GetAllActions();

        return new Dictionary<string, Feature>() {
            {"river", new Feature("river", Feature.FeatureType.river, UNSET, 2,
                                    new List<GenericAction>(){
                                        actions["fish"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"common_fish", new List<string>(){"trout", "bass"} },
                                        {"rare_fish", new List<string>(){"salmon"} }
                                    })
            },
            {"door", new Feature("door", Feature.FeatureType.door, UNSET, 100,
                                    new List<GenericAction>(){ actions["move"] },
                                    new Dictionary<string, List<string>>())
            },
            {"shop", new Feature("shop", Feature.FeatureType.shop, UNSET, 2,
                                    new List<GenericAction>(){ //ActionInitializer.actions["buy_#item#"],
                                                                //ActionInitializer.actions["outing_shopping_at_#loc#_with_#b#"]
                                    },
                                    new Dictionary<string, List<string>>(),
                                    new Dictionary<string, int>(){})
            },
            {"kitchen", new Feature("kitchen", Feature.FeatureType.kitchen, UNSET, 2,
                                    new List<GenericAction>(ActionInitializer.GenerateRecipes().Values),
                                    new Dictionary<string, List<string>>(),
                                    new Dictionary<string, int>(){})
            },
            {"brush", new Feature("brush", Feature.FeatureType.brush, UNSET, 2,
                                    new List<GenericAction>(){
                                        actions["forage"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"common_forage", new List<string>(){"blackberry"} },
                                        {"rare_forage", new List<string>(){"strawberry"} }
                                    })
            },
            {"meadow", new Feature("meadow", Feature.FeatureType.meadow, UNSET, 2,
                                    new List<GenericAction>(){
                                        actions["forage"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"common_forage", new List<string>(){"tulip", "rose"} },
                                        {"rare_forage", new List<string>(){"morning_rose", "evening_tulip", "herb"} }
                                    })
            },
            {"cow", new Feature("cow", Feature.FeatureType.animal, UNSET, 2,
                                    new List<GenericAction>(){
                                        actions["tend_animal"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"produce", new List<string>(){ "milk"} }
                                    })
            },
            {"chicken", new Feature("chicken", Feature.FeatureType.animal, UNSET, 2,
                                    new List<GenericAction>(){
                                        actions["tend_animal"]
                                    },
                                    new Dictionary<string, List<string>>() {
                                        {"produce", new List<string>(){ "egg"} }
                                    })
            },
            {"field", new Feature("field", Feature.FeatureType.field, UNSET, 2,
                                    new List<GenericAction>( ){
                                        actions["harvest_crops"],
                                        actions["tend_crops"]
                                    },
                                    new Dictionary<string, List<string>>(){
                                        {"planted", ItemInitializer.plantableCrops}
                                    } )
            },
            {"SYSTEM", new Feature("SYSTEM", Feature.FeatureType.SYSTEM, UNSET, 100,
                                    new List<GenericAction>(){
                                        
                                    },
                                    new Dictionary<string, List<string>>() {
                                        
                                    })
            }
        };
    }

    static Dictionary<string, List<string>> featuresInRoom = new Dictionary<string, List<string>>() {
        {"farm", new List<string>(){"river", "chicken", "cow", "field" } },
        {"field", new List<string>(){ "river", "meadow"} },
        {"forest", new List<string>(){"brush" } },
        {"inn", new List<string>(){"kitchen"} },
        {"averysHouse", new List<string>(){"kitchen"} },
        {"sammysHouse", new List<string>(){"kitchen"} },
        {"finleysHouse", new List<string>(){"kitchen"} },
        {"SYSTEM", new List<string>{"SYSTEM" } }
    };

    static List<KeyValuePair<string, string>> roomConnections = new List<KeyValuePair<string, string>>() {
        new KeyValuePair<string, string>("farm", "field"),
        new KeyValuePair<string, string>("field", "hill"),
        new KeyValuePair<string, string>("field", "forest"),
        new KeyValuePair<string, string>("field", "town"),
        new KeyValuePair<string, string>("town", "inn"),
        new KeyValuePair<string, string>("farm", "averysHouse"),
        new KeyValuePair<string, string>("town", "sammysHouse"),
        new KeyValuePair<string, string>("field", "finleysHouse"),
    };


    static List<string> allLocationNames = new List<string>() {
        "farm",
        "field",
        "town",
        "forest",
        "inn",
        "averysHouse",
        "sammysHouse",
        "finleysHouse",
        "hill",
        "SYSTEM"
    };


    public static Dictionary<string, Feature> GetAllFeatures()
    {
        Dictionary<string, Feature> allFeatures = new Dictionary<string, Feature>();

        foreach(string room in featuresInRoom.Keys) {
            foreach(string ft in featuresInRoom[room]) {
                Feature newFeature = MakeFeatures()[ft];
                newFeature.location = room;
                newFeature.id += "_" + newFeature.location;

                allFeatures.Add(newFeature.id, newFeature);
            }
        }


        List<Feature> uniqueFetures = MakeUniqueShops();

        foreach(Feature feature in uniqueFetures) {
            allFeatures.Add(feature.id, feature);
        }


        foreach (KeyValuePair<string, string> connection in roomConnections) {
            Feature door = MakeFeatures()["door"];
            door.location = connection.Key;
            door.relevantResources.Add(Map.R_CONNECTEDLOCATION, new List<string>() { connection.Value });
            door.id += "_" + door.location + "->" + connection.Value;

            allFeatures.Add(door.id, door);

            door = MakeFeatures()["door"];
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
        Dictionary<string, Location> allLocations = new Dictionary<string, Location>();

        foreach(string loc in allLocationNames) {
            allLocations.Add(loc, new Location(loc, new Dictionary<string, List<string>>()));
        }

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

    static Feature MakeShop(string id, string location, List<string> actions)
    {
        Feature shop = MakeFeatures()["shop"];
        shop.id = id;
        shop.location = location;

        foreach(string actionId in actions) {
            shop.providedActions.Add(ActionInitializer.actions[actionId]);
        }

        return shop;
    }

    static List<Feature> MakeUniqueShops()
    {
        List<Feature> features = new List<Feature>() {
            MakeShop("tackle_shop_town", "town", new List<string>() {
                "buy_fishing_rod",
                "sell_fish_at_market"
            }),
            MakeShop("farmers_market_town", "town", new List<string>() {
                "sell_crop_at_market"
            }),
        };


        return features;
    }
}
