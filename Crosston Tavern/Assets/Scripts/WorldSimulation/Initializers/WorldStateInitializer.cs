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

    static public List<Townie> GetTownies( WorldState ws, Transform townieHolder)
    {
        List<Townie> townies = new List<Townie>();

        foreach (Person person in ws.registry.GetPeople()) {
            WorldState personalWorldState = ws.Copy(person, person.id);

            GameObject townieObj = new GameObject(person.id);
            townieObj.AddComponent<Townie>();
            townieObj.GetComponent<Townie>().TownieInit(person, personalWorldState, new GoalManager(personalWorldState, person));

            townieObj.transform.parent = townieHolder; 

            townies.Add(townieObj.GetComponent<Townie>());
                //new Townie(person, ws, new GoalManager(ws, person)));
                //new Townie(person, personalWorldState, new GoalManager(personalWorldState, person)));
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
        townies[1].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[1].townieInformation.id, "strawberry_cake", 1, 100), 4)
                                    }
                                ));

        townies[2].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateSocial("clara", "organizer_alicia", Relationship.RelationType.friendly, -100, -5), 1)
                                    }
                                ));



        townies[0].ws.knownFacts.AddFact(new WorldFactResource("river_farm", "common_fish", "trout"), townies[0].ws);
        townies[0].ws.knownFacts.AddFact(new WorldFactResource("river_feild", "common_fish", "trout"), townies[0].ws);
        townies[1].ws.knownFacts.AddFact(new WorldFactResource("brush_forest", "rare_forage", "strawberry"), townies[1].ws);


        return townies;
    }
}
