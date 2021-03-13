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
            PeopleInitializer.SetInitialRelationTags(townie);

            townies.Add(townie);

            townie.homeLocation = townie.townieInformation.location;
        }

        int barkeep = 0;
        int sammy = 2;
        int avery = 1;
        int finley = 3;

        foreach(Townie townie in townies) {
            AddGeneralGoals(townie);
        }


        townies[sammy].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        //new GoalState(new StateInventoryStatic(townies[sammy].townieInformation.id, "trout", 20, 100), 1),
                                        new GoalState(new StateSkill(townies[sammy].townieInformation.id, "fishing", 90, 100), (float)GoalManager.GoalPriority.medium)
                                    },
                                    "I want to be a better fisher."
                                )) ;
        //townies[sammy].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new GoalState(new StateInventoryStatic(townies[sammy].townieInformation.id, "strawberry_cake", 1,100), 1)
        //                            },
        //                            "I love cake."
        //                        ));


        //townies[avery].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new GoalState(new StateInventoryStatic(townies[avery].townieInformation.id, "blackberry_tart", 1,100), 1)
        //                            },
        //                            "I love tarts."
        //                        ));

        townies[finley].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new GoalState(new StateInventoryStatic(townies[finley].townieInformation.id, "blackberry", 20, 100), 1)
                                    },
                                    "I like eating blackberries."
                                ));
        //townies[finley].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new GoalState(new StateInventoryStatic(townies[finley].townieInformation.id, "blackberry_tart", 1,100), 1)
        //                            },
        //                            "I love tarts."
        //                        ));


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


    static void AddGeneralGoals(Townie townie)
    {
        GoalManager gm = townie.gm;
        string name = townie.Id;

        Dictionary<Relationship.Tag, GoalManager.GoalPriority> datingStatePriority =
            new Dictionary<Relationship.Tag, GoalManager.GoalPriority>() {
                {Relationship.Tag.crushing_on, GoalManager.GoalPriority.low },
                {Relationship.Tag.in_love_with, GoalManager.GoalPriority.medium },
                {Relationship.Tag.head_over_heels, GoalManager.GoalPriority.high },
            };

        Dictionary<Relationship.Tag, GoalManager.GoalPriority> friendStatePriority =
            new Dictionary<Relationship.Tag, GoalManager.GoalPriority>() {
                {Relationship.Tag.liked, GoalManager.GoalPriority.low },
                {Relationship.Tag.friend, GoalManager.GoalPriority.low },
                {Relationship.Tag.bestFriend, GoalManager.GoalPriority.medium },
            };


        foreach (string otherPerson in townie.townieInformation.relationships.GetKnownPeople()) {
            foreach (KeyValuePair<Relationship.Tag, GoalManager.GoalPriority> pair in datingStatePriority) {
                gm.AddModule(
                    new GoalModule(
                        new List<GM_Precondition>() {
                            new GM_Precondition_State(new StateRelation(name, otherPerson, pair.Key)),
                            new GM_Precondition_TopRelationAxis(Relationship.Axis.romantic, name, otherPerson)
                        },
                        new List<Goal>() {
                            new GoalState(new StateRelation(name, otherPerson, Relationship.Tag.dating), (int)pair.Value)
                        },
                        name: pair.Key + otherPerson
                    )
                );
            }
        }
        foreach (string otherPerson in townie.townieInformation.relationships.GetKnownPeople()) {
            foreach (KeyValuePair<Relationship.Tag, GoalManager.GoalPriority> pair in friendStatePriority) {
                gm.AddModule(
                    new GoalModule(
                        new List<GM_Precondition>() {
                            new GM_Precondition_State(new StateRelation(name, otherPerson, pair.Key))
                        },
                        new List<Goal>() {
                            new GoalState(new StateRelation( otherPerson, name, pair.Key), (int)pair.Value)
                        },
                        name: pair.Key + otherPerson
                    )
                );
            }
        }

        foreach (string item in townie.townieInformation.preference.preferences[PreferenceLevel.loved]) {
            GenericAction eat = ActionInitializer.GetAllActions()["eat_#item#"];
            gm.AddModule(
                new GoalModule(
                    new List<GM_Precondition>() {
                    },
                    new List<Goal>() {
                        new GoalAction(new BoundAction(eat, name, name, "",
                                                            new BoundBindingCollection( new List<BoundBindingPort>() {
                                                                new BoundBindingPort("item", item)
                                                            }),
                                                            eat.verbilizationInfo
                                                       ),
                                        (int)GoalManager.GoalPriority.medium
                        )
                    },
                    name: "loves to eat " + item
                )
            );
        }

       
        

        
        

        
    }
}


public class ResourceCatagories
{
    public static string r_connectedLocation = "connectedLocation";

    public static string r_initiator = "initiator";
    public static string r_recipient = "recipient";

}