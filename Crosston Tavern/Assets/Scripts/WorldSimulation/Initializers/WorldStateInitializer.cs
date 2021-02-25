using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateInitializer 
{
    static public WorldState GetWorldState()
    {
        Map map = new Map(new List<Feature>(FeatureInitializer.GetAllFeatures().Values),
                          new List<Location>(FeatureInitializer.GetAllLocations().Values));

        return new WorldState(map, WorldTime.DayZeroEightAM, "global");
    }

    static public List<Townie> GetTownies( WorldState globalWs, Transform townieHolder)
    {
        List<Townie> townies = new List<Townie>();

        foreach (Person person in globalWs.map.GetPeople()) {
            WorldState personalWorldState = globalWs.Copy((Person)person.Copy(true), person.id);

            //Make Each Townie their own Game Object for debugging purposes:
            GameObject townieObj = new GameObject(person.id);
            townieObj.transform.parent = townieHolder; 
            Townie townie = townieObj.AddComponent<Townie>();
            townie.TownieInit(person.id, personalWorldState, new GoalManager(personalWorldState, person));

            PeopleInitializer.SetAssumedPerceptionsOfOthers(townie);

            townies.Add(townie);

            townie.homeLocation = townie.townieInformation.location;
        }

        int barkeep = 0;
        int sammy = 2;
        int avery = 1;
        int finley = 3;


        townies[sammy].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new GoalState(new StateInventoryStatic(townies[sammy].townieInformation.id, "trout", 20, 100), 1),
                                        new GoalState(new StateSkill(townies[sammy].townieInformation.id, "fishing", 90, 100), 1)
                                    },
                                    "I want to be a better fisher."
                                )) ;
        //townies[sammy].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new GoalState(new StateInventoryStatic(townies[sammy].townieInformation.id, "trout", 20, 100), 1)
        //                            },
        //                            "I want to be a better fisher."
        //                        ));

        townies[avery].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new GoalState(new StateRelation("avery", "sammy", Relationship.RelationshipTag.dating), 1)
                                    },
                                    "I think they're so cool!"
                                ));
        //townies[avery].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new GoalState(new StateInventoryStatic(townies[avery].townieInformation.id, "flour", 5, 100), 1),
        //                            }
        //                        ));


        /*townies[clara].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateSocial("clara", "alicia", Relationship.RelationType.friendly, -100, -5), 1)
                                    }
                                ));*/
        townies[finley].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new GoalState(new StateInventoryStatic(townies[finley].townieInformation.id, "blackberry", 20, 100), 1)
                                    },
                                    "I like eating blackberries."
                                ));


        townies[sammy].ws.knownFacts.AddFact(new WorldFactResource("river_farm", "common_fish", "trout"), townies[sammy].ws);
        townies[sammy].ws.knownFacts.AddFact(new WorldFactResource("river_field", "common_fish", "trout"), townies[sammy].ws);
        townies[avery].ws.knownFacts.AddFact(new WorldFactResource("chicken_farm", "produce", "egg"), townies[avery].ws);
        townies[avery].ws.knownFacts.AddFact(new WorldFactResource("cow_farm", "produce", "milk"), townies[avery].ws);
        foreach(string item in ItemInitializer.plantableCrops) {
            townies[avery].ws.knownFacts.AddFact(new WorldFactResource("field_farm", "planted", item), townies[avery].ws);
        }
        townies[finley].ws.knownFacts.AddFact(new WorldFactResource("brush_forest", "common_forage", "blackberry"), townies[finley].ws);

        return townies;
    }
}


public class ResourceCatagories
{
    public static string r_connectedLocation = "connectedLocation";

    public static string r_initiator = "initiator";
    public static string r_recipient = "recipient";

}