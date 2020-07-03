using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateInitializer 
{
    static public WorldState GetWorldState()
    {
        Map map = new Map(new List<Feature>(FeatureInitializer.GetAllFeatures().Values),
                          new List<Location>(FeatureInitializer.GetAllLocations().Values));
        Registry registry = new Registry(map.GetAllFeatures());

        return new WorldState(map, registry, WorldTime.DayZeroEightAM, "global");
    }

    static public List<Townie> GetTownies( WorldState ws)
    {
        List<Townie> townies = new List<Townie>();

        foreach (Person person in ws.registry.GetPeople()) {
            WorldState personalWorldState = ws.Copy(person, person.id);

            townies.Add(//new Townie(person, ws, new GoalManager(ws, person)));
                        new Townie(person, personalWorldState, new GoalManager(personalWorldState, person)));
        }


        townies[0].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[0].townieInformation.id, "trout", 20, 100), 1)
                                    }
                                ));
        townies[1].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateRelation("lover_bob", "organizer_alicia", Relationship.RelationshipTag.dating), 1)
                                    }
                                )) ;

        return townies;
    }
}
