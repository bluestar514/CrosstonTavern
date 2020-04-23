using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[System.Serializable]
//public class WorldInitializer
//{

//    protected virtual List<Location> InitializeLocations()
//    {
//        List<Location> locations = new List<Location>() {
//            new Location("farm",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "town"} }
//                }
//            ),
//            new Location("forest",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"salmon", "tuna"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"farm", "town", "f1", "caves"} }
//                }
//            ),
//            new Location("town",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "farm", "inn", "caves"} }
//                }
//            ),
//            new Location("inn",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"town"} }
//                }
//            ),
//            new Location("f1",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "f2","f3"} }
//                }
//            ),
//            new Location("f2",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f1", "f4"} }
//                }
//            ),
//            new Location("f3",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f1", "f4"} }
//                }
//            ),
//            new Location("f4",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f2", "f4"} }
//                }
//            ),
//            new Location("caves",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "town", "c1", "c2", "c3"} }
//                }
//            ),
//            new Location("c1",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c4"} }
//                }
//            ),
//            new Location("c2",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c5"} }
//                }
//            ),
//            new Location("c3",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c5"} }
//                }
//            ),
//            new Location("c4",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c1", "c8"} }
//                }
//            ),
//            new Location("c5",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c2", "c3", "c6", "c7"} }
//                }
//            ),
//            new Location("c6",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c5", "c8"} }
//                }
//            ),
//            new Location("c7",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c5"} }
//                }
//            ),
//            new Location("c8",
//                new Dictionary<string, List<string>>() {
//                    {"fish", new List<string>(){"trout", "goldfish"} },
//                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c6", "c4"} }
//                }
//            )
//        };


//        return locations;
//    }

//    public virtual List<Person> InitializePeople()
//    {
//        List<Person> people = new List<Person>() {
//            new Person("Alicia"),
//            new Person("Bob"),
//            new Person("Clara"),
//            new Person("Darel"),
//            new Person("Everet"),
//            new Person("Faraz"),
//            new Person("Gigi"),
//            new Person("Howard")
//        };

//        return people;
//    }

//    public virtual Dictionary<string, GenericAction> InitializeActions()
//    {
//        Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>() {
//            {
//                "fish",
//                new GenericAction("fish", 3,
//                    new List<Condition>(){
//                        new Condition_SpaceAtFeature(),
//                        new Condition_IsState(new InvChange(1, 10000, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"fishing_rod" }))
//                    },
//                    new List<Effect>() {
//                        new Effect(
//                            new ChanceModifierSimple(.5f),
//                            new List<MicroEffect>(){
//                                new InvChange(1,1, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"#fish#" })
//                            }
//                        ),
//                        new Effect(
//                            new ChanceModifierSimple(.25f),
//                            new List<MicroEffect>(){
//                                new InvChange(1,1, "#"+ResourceCatagories.r_initiator+"#",new List<string>(){"old boot"})
//                            }
//                        ),
//                        new Effect(
//                            new ChanceModifierSimple(.25f),
//                            new List<MicroEffect>(){
//                                new InvChange(1,3, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"algee" })
//                            }
//                        )
//                    }
//                )
//            },
//            {
//                "buy_fishing_rod",
//                new GenericAction("buy_fishing_rod", 0, 
//                    new List<Condition>(){
//                        new Condition_IsState(new InvChange(10, 10000, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"currency" }))
//                    }, 
//                    new List<Effect>() {
//                        new Effect( 
//                            new ChanceModifier(), 
//                            new List<MicroEffect>() {
//                                new InvChange(1, 1, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"fishing_rod" }),
//                                new InvChange(-10, -10, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"currency" })

//                            })
//                    })
//            },
//            {
//                "move",
//                new GenericAction("move", 0,
//                    new List<Condition>(){ new Condition_SpaceAtFeature()},
//                    new List<Effect>() {
//                        new Effect(
//                            new ChanceModifier(),
//                            new List<MicroEffect>(){
//                                new Move()
//                            }
//                        )
//                    }
//                )
//            },
//            {
//                "forage",
//                new GenericAction("forage", 2,
//                    new List<Condition>(){ new Condition_SpaceAtFeature()},
//                    new List<Effect>() {
//                        new Effect(
//                            new ChanceModifierSimple(.5f),
//                            new List<MicroEffect>() {
//                                new InvChange(1, 5, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"#mushroom#", "#herb#" })
//                            }
//                        ),
//                        new Effect(
//                            new ChanceModifierSimple(.25f),
//                            new List<MicroEffect>() {
//                                new InvChange(1, 3, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"#flower#" })
//                            }
//                        ),
//                        new Effect(
//                            new ChanceModifierSimple(.25f),
//                            new List<MicroEffect>() {
//                                new InvChange(1, 5, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"#poisonous_mushroom#" })
//                            }
//                        )
//                    }
//                )
//            },
//            {
//                "talk",
//                new GenericAction("talk", 1, new List<Condition>(){ new Condition_NotYou(), new Condition_SpaceAtFeature()},
//                    new List<Effect>(){
//                        new Effect(
//                            new ChanceModifier(),
//                            new List<MicroEffect>() {
//                                new SocialChange(1,1, "#"+ResourceCatagories.r_initiator+"#", "#"+ResourceCatagories.r_recipient+"#"),
//                                new SocialChange(1,1, "#"+ResourceCatagories.r_recipient+"#", "#"+ResourceCatagories.r_initiator+"#"),
//                            }
//                        )
//                    }
//                )
                
//            }
//        };

//        return actions;
//    }

//    protected virtual List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, List<Location> locations, List<Person> people)
//    {
//        List<Feature> features = new List<Feature>() {
//            new Feature("farm_river", "farm", 2,
//                new List<GenericAction>(){
//                    actions["fish"]
//                },
//                new Dictionary<string, List<string>>(){
//                    {"fish", new List<string>(){ "#fish#", "smelt"} }
//                }
//            ),
//            new Feature("forest_river", "forest", 2,
//                new List<GenericAction>(){
//                    actions["fish"]
//                },
//                new Dictionary<string, List<string>>(){
//                    {"fish", new List<string>(){ "#fish#", "silverfish"} }
//                }
//            ),
            
//        };

//        AddDoors(features, actions, locations);

//        AddPeopleToFeatures(features, people, actions);

//        return features;
//    }

//    protected void AddDoors(List<Feature> features, Dictionary<string, GenericAction> actions, List<Location> locations)
//    {
//        foreach (Location location in locations) {
//            List<Feature> doors = new List<Feature>(
//                    from loc in location.resources["connectedLocation"]
//                    select new Feature("door_" + location.Id + "->" + loc,
//                                        location.Id, 10000,
//                                        new List<GenericAction>() {
//                                            actions["move"]
//                                        },
//                                        new Dictionary<string, List<string>>(){
//                                            {Map.R_CONNECTEDLOCATION, new List<string>(){loc} }
//                                        }
//                                    )
//                    );

//            features.AddRange(doors);
//        }
//    }

//    protected void AddPeopleToFeatures(List<Feature> features, List<Person> people, Dictionary<string, GenericAction> actions)
//    {
//        foreach (Person person in people) {
//            person.location = "farm";
//            person.feature.providedActions.Add(actions["talk"]);
//            person.feature.location = person.location;

//            features.Add(person.feature);
//        }
//    }

//    public void AddGoalsToPeople(WorldState ws)
//    {
//        Registry reg = ws.registry;


//        Person person = reg.GetPerson("Alicia");
//        SetPlaceOfWork(person, ws, "town_fishShop", new WorldTime(-1, -1, -1, 8, 30), new WorldTime(-1, -1, -1, 15, 50));

//        person = reg.GetPerson("Bob");
//        SetPlaceOfWork(person, ws, "town_herbShop", new WorldTime(-1, -1, -1, 9, 00), new WorldTime(-1, -1, -1, 14, 50));

//        person = reg.GetPerson("Clara");
//        SetPlaceOfWork(person, ws, "town_inn", new WorldTime(-1, -1, -1, 7, 00), new WorldTime(-1, -1, -1, 13, 50));

//        person = reg.GetPerson("Darel");
//        SetPlaceOfWork(person, ws, "town_fishShop", new WorldTime(-1, -1, -1, 12, 20), new WorldTime(-1, -1, -1, 18, 00));

//        person = reg.GetPerson("Everet");
//        SetPlaceOfWork(person, ws, "town_herbShop", new WorldTime(-1, -1, -1, 12, 00), new WorldTime(-1, -1, -1, 17, 50));

//        person = reg.GetPerson("Faraz");
//        SetPlaceOfWork(person, ws, "town_inn", new WorldTime(-1, -1, -1, 12, 00), new WorldTime(-1, -1, -1, 20, 50));

//        person = reg.GetPerson("Gigi");
//        SetPlaceOfWork(person, ws, "town_inn", new WorldTime(-1, -1, -1, 12, 00), new WorldTime(-1, -1, -1, 20, 50));

//        person = reg.GetPerson("Howard");
//        SetPlaceOfWork(person, ws, "town_inn", new WorldTime(-1, -1, -1, 12, 00), new WorldTime(-1, -1, -1, 20, 50));
//    }

//    protected void SetPlaceOfWork(Person person, WorldState ws, string featureWorkPlace, WorldTime shiftStart, WorldTime shiftEnd)
//    {
//        if(person.gm == null) person.gm = new GoalManager(ws, person);

//        person.placeOfWork = featureWorkPlace;//"town_fishShop";

//        person.gm.AddModule(new GM_ManLocation(
//                                    new List<GM_Precondition>() {
//                                        new GM_Precondition_Time(shiftStart, shiftEnd)
//                                    },
//                                    new List<Goal>() { },
//                                    ws.map.GetFeature(person.placeOfWork).location));
//        person.gm.AddModule(new GM_StockFeature(
//                                    new List<GM_Precondition>() {
//                                        new GM_Precondition_Time(shiftEnd, shiftStart)
//                                    },
//                                    new List<Goal>() { },
//                                    person.Id,
//                                    person.placeOfWork, ws));
//    }

//    public virtual Map InitializeMap(List<Person> people)
//    {
//        List<Location> locations = InitializeLocations();
//        List<Feature> features = InitializeFeatures(InitializeActions(), locations, people);

//        return new Map(features, locations);
//    }

//    public virtual Registry InitializeRegistry(List<Person> people)
//    {

//        foreach(Person person in people) {
//            person.ChangeInventoryContents(10, "currency");
//        }

//        return new Registry(people);
//    }
//}

public class ResourceCatagories
{
    public static string r_connectedLocation = "connectedLocation";

    public static string r_initiator = "initiator";
    public static string r_recipient = "recipient";

}