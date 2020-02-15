using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldInitializer
{
    List<Location> InitializeLocations()
    {
        List<Location> locations = new List<Location>() {
            new Location("farm",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "town"} }
                }
            ),
            new Location("forest",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"salmon", "tuna"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"farm", "town", "f1", "caves"} }
                }
            ),
            new Location("town",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "farm", "inn", "caves"} }
                }
            ),
            new Location("inn",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"town"} }
                }
            ),
            new Location("f1",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "f2","f3"} }
                }
            ),
            new Location("f2",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f1", "f4"} }
                }
            ),
            new Location("f3",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f1", "f4"} }
                }
            ),
            new Location("f4",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"f2", "f4"} }
                }
            ),
            new Location("caves",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"forest", "town", "c1", "c2", "c3"} }
                }
            ),
            new Location("c1",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c4"} }
                }
            ),
            new Location("c2",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c5"} }
                }
            ),
            new Location("c3",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"caves", "c5"} }
                }
            ),
            new Location("c4",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c1", "c8"} }
                }
            ),
            new Location("c5",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c2", "c3", "c6", "c7"} }
                }
            ),
            new Location("c6",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c5", "c8"} }
                }
            ),
            new Location("c7",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c5"} }
                }
            ),
            new Location("c8",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {Map.R_CONNECTEDLOCATION, new List<string>(){"c6", "c4"} }
                }
            )
        };


        return locations;
    }

    public List<Person> InitializePeople()
    {
        List<Person> people = new List<Person>() {
            new Person("Alicia"),
            new Person("Bob"),
            new Person("Clara"),
            new Person("Darel"),
            new Person("Everet"),
            new Person("Faraz"),
            new Person("Gigi"),
            new Person("Howard")
        };


        return people;
    }

    public Dictionary<string, GenericAction> InitializeActions()
    {
        Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>() {
            {
                "fish",
                new GenericAction("fish",
                    new List<Condition>(),
                    new List<Effect>() {
                        new Effect(
                            new ChanceModifierSimple(.4f),
                            new List<MicroEffect>(){
                                new InvChange(1,1, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"#fish#" })
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,1, "#"+ResourceCatagories.r_initiator+"#",new List<string>(){"old boot"})
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,3, "#"+ResourceCatagories.r_initiator+"#", new List<string>(){"algee" })
                            }
                        )
                    }
                )
            },
            {
                "move",
                new GenericAction("move",
                    new List<Condition>(),
                    new List<Effect>() {
                        new Effect(
                            new ChanceModifier(),
                            new List<MicroEffect>(){
                                new Move()
                            }
                        )
                    }
                )
            },
            {
                "talk",
                new GenericAction("talk", new List<Condition>(),
                    new List<Effect>(){
                        new Effect(
                            new ChanceModifier(),
                            new List<MicroEffect>() {
                                new SocialChange(1,1, "#"+ResourceCatagories.r_initiator+"#", "#"+ResourceCatagories.r_recipient+"#"),
                                new SocialChange(1,1, "#"+ResourceCatagories.r_recipient+"#", "#"+ResourceCatagories.r_initiator+"#"),
                            }
                        )
                    }
                )
                
            }
        };

        return actions;
    }

    List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, List<Location> locations, List<Person> people)
    {
        List<Feature> features = new List<Feature>() {
            new Feature("farm_river", "farm",
                new List<GenericAction>(){
                    actions["fish"]
                },
                new Dictionary<string, List<string>>(){
                    {"fish", new List<string>(){ "#fish#", "smelt"} }
                }
            ),
            new Feature("forest_river", "forest",
                new List<GenericAction>(){
                    actions["fish"]
                },
                new Dictionary<string, List<string>>(){
                    {"fish", new List<string>(){ "#fish#", "silverfish"} }
                }
            ),
            
        };

        foreach( Location location in locations) {
            List<Feature> doors = new List<Feature>(
                    from loc in location.resources["connectedLocation"]
                    select new Feature("door_" + location.Id + "->" + loc, 
                                        location.Id,
                                        new List<GenericAction>() {
                                            actions["move"]
                                        },
                                        new Dictionary<string, List<string>>(){
                                            {Map.R_CONNECTEDLOCATION, new List<string>(){loc} }
                                        }
                                    )
                    );

            features.AddRange(doors);
        }



        foreach (Person person in people) {
            person.location = "farm";
            person.feature.providedActions.Add(actions["talk"]);
            person.feature.location = person.location;

            features.Add(person.feature);

            person.goalPriorityDict = new Dictionary<MicroEffect, float>() {
                {new Move("forest"), 1 }
            };
        }


        return features;
    }

    public Map InitializeMap(List<Person> people)
    {
        List<Location> locations = InitializeLocations();
        List<Feature> features = InitializeFeatures(InitializeActions(), locations, people);

        return new Map(features, locations);
    }

    public Registry InitializeRegistry(List<Person> people)
    {
        return new Registry(people);
    }
}

public class ResourceCatagories
{
    public static string r_connectedLocation = "connectedLocation";

    public static string r_initiator = "initiator";
    public static string r_recipient = "recipient";

}