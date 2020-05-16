using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateInitializer 
{
    static public WorldState GetWorldState()
    {
        return new WorldState(new Map(
                                    new List<Feature>(FeatureInitializer.GetAllFeatures().Values),
                                    new List<Location>(FeatureInitializer.GetAllLocations().Values)),
                            new Registry(new List<Person>(PeopleInitializer.GetAllPeople().Values)),
                            WorldTime.DayZeroEightAM);
    }

    static public List<Townie> GetTownies()
    {
        List<Townie> townies = new List<Townie>();

        foreach(Person person in PeopleInitializer.GetAllPeople().Values) {
            WorldState ws = GetWorldState();


            townies.Add(new Townie(person, ws, new GoalManager(ws, person)));
        }

        return townies;
    }
}
