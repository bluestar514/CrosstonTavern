﻿using System.Collections;
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

    static public List<Townie> GetTownies( WorldState ws = null)
    {
        List<Townie> townies = new List<Townie>();

        foreach(Person person in PeopleInitializer.GetAllPeople().Values) {
            if(ws == null) ws = GetWorldState();


            townies.Add(new Townie(person, ws, new GoalManager(ws, person)));
        }


        townies[0].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventory(townies[0].townieInformation.id, "trout", 5, 10), 1)
                                    }
                                ));
        townies[1].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateSocial(townies[1].townieInformation.id, "alicia", Relationship.RelationType.friendly, 5, 10), 1)
                                    }
                                ));

        return townies;
    }
}