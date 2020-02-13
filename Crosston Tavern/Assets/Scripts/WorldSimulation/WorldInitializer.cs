using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInitializer
{
    public Dictionary<string, Location> InitializeLocations()
    {
        Dictionary<string, Location> locations = new Dictionary<string, Location>() {
            {"farm", new Location("farm",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"trout", "goldfish"} },
                    {"connectedSpaces", new List<string>(){"forest"} }
                }
            ) },
            {"forest", new Location("forest",
                new Dictionary<string, List<string>>() {
                    {"fish", new List<string>(){"salmon", "tuna"} },
                    {"connectedSpaces", new List<string>(){"farm"} }
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
                                new InvChange(1,1,"Feature.fish")
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,1,"old boot")
                            }
                        ),
                        new Effect(
                            new ChanceModifierSimple(.5f),
                            new List<MicroEffect>(){
                                new InvChange(1,3,"algee")
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

    public List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions)
    {
        List<Feature> features = new List<Feature>() {
            new Feature("farm_river", "farm",
                new List<GenericAction>(){
                    actions["fish"]
                },
                new Dictionary<string, List<string>>(){
                    {"fish", new List<string>(){ "#Location.fish", "smelt"} }
                }
            ),
            new Feature("forest_river", "forest",
                new List<GenericAction>(){
                    actions["fish"]
                },
                new Dictionary<string, List<string>>(){
                    {"fish", new List<string>(){ "#Location.fish", "silverfish"} }
                }
            ),
            new Feature("door_farm", "farm",
                new List<GenericAction>() {
                    actions["move"]
                },
                new Dictionary<string, List<string>>(){ }
            ),
            new Feature("door_forest", "forest",
                new List<GenericAction>() {
                    actions["move"]
                },
                new Dictionary<string, List<string>>(){ }
            )
        };

        return features;
    }
}
