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

    static public List<Townie> GetTownies( WorldState ws)
    {
        List<Townie> townies = new List<Townie>();

        foreach (Person person in ws.registry.GetPeople()) {

            townies.Add(new Townie(person, ws, new GoalManager(ws, person)));
        }


        townies[0].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[0].townieInformation.id, "trout", 5, 10), 1)
                                    }
                                ));
        townies[1].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        //new Goal(new StateSocial(townies[1].townieInformation.id, "alicia", Relationship.RelationType.friendly, 5, 10), 1)
                                        new Goal(new StateRelation("bob", "alicia", Relationship.RelationshipTag.dating), 1)
                                    }
                                )) ;

        return townies;
    }
}
