using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldInitializer
{
    public Dictionary<string, Location> InitializeLocations()
    {
        Dictionary<string, Location> locations = new Dictionary<string, Location>() {
            {"farm", new Location("farm",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {"connectedLocation", new List<string>(){"forest"} }
                }
            ) },
            {"forest", new Location("forest",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"salmon", "tuna"} },
                    {"connectedLocation", new List<string>(){"farm"} }
                }
            ) }
        };


        return locations;
    }

    public List<Person> InitializePeople(List<Feature> features, Dictionary<string, GenericAction> actions)
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

        foreach (Person person in people) {
            person.location = "farm";
            person.feature = new Feature("person_" + person.Id, person.location,
                new List<GenericAction>() { actions["talk"] },
                new Dictionary<string, List<string>> {
                    {"recipient", new List<string>{person.Id} }
                }
            );

            features.Add(person.feature);

            person.goalPriorityDict = new Dictionary<MicroEffect, float>() {
                {new Move("forest"), 1 }
            };
        }

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
                                new InvChange(1,1,new List<string>(){"#fish#" })
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,1,new List<string>(){"old boot"})
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,3,new List<string>(){"algee" })
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
                                new SocialChange(1,1,"#initiator#", "#recipient#"),
                                new SocialChange(1,1, "#recipient#", "#initiator#"),
                            }
                        )
                    }
                )
                
            }
        };

        return actions;
    }

    public List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, Dictionary<string, Location> locations)
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

        foreach( Location location in locations.Values) {
            List<Feature> doors = new List<Feature>(
                    from loc in location.resources["connectedLocation"]
                    select new Feature("door_" + location.Id + "->" + loc, location.Id,
                                    new List<GenericAction>() {
                                        actions["move"]
                                    },
                                    new Dictionary<string, List<string>>(){
                                        {"connectedLocation", new List<string>(){loc} }
                                    }
                                )
                    );
        }

        return features;
    }
}
