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
            Townie townie = townieObj.AddComponent<Townie>();
            townie.TownieInit(person, personalWorldState, new GoalManager(personalWorldState, person));

            string self = townie.townieInformation.id;
            foreach (Person other in townie.ws.registry.GetPeople()) {
                List<Relationship.RelationshipTag> relationshipTag = person.relationships.GetTag(other.id);

                foreach(Relationship.RelationshipTag tag in relationshipTag) {
                    if(new List<Relationship.RelationshipTag>() {
                        Relationship.RelationshipTag.acquantences,
                        Relationship.RelationshipTag.enemies,
                        Relationship.RelationshipTag.friends,
                        Relationship.RelationshipTag.lovers
                        }.Contains(tag)) {

                        foreach (Relationship.RelationType relationType in new List<Relationship.RelationType>()
                                            { Relationship.RelationType.friendly, Relationship.RelationType.romantic}) {
                            float[] range = Relationship.codifiedRelationRanges[tag][relationType];
                            float value = Random.Range(Mathf.Max(range[0], -6), Mathf.Min(range[1], 10));

                            other.relationships.Increase(self, relationType, value);
                        }
                    }
                }
            }

            townieObj.transform.parent = townieHolder; 

            townies.Add(townieObj.GetComponent<Townie>());
            //new Townie(person, ws, new GoalManager(ws, person)));
            //new Townie(person, personalWorldState, new GoalManager(personalWorldState, person)));

            townieObj.GetComponent<Townie>().homeLocation = "farm";
        }

        int barkeep = 0;
        int alicia = 1;
        int bob = 2;
        int clara = 3;
        int dirk = 4;

        townies[barkeep].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StatePosition("barkeep", "inn"), 100)
                                    }
                                ));
        townies[alicia].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[alicia].townieInformation.id, "trout", 20, 100), 1)
                                    }
                                ));

        townies[bob].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateRelation("bob", "alicia", Relationship.RelationshipTag.dating), 1)
                                    }
                                )) ;
        //townies[bob].gm.AddModule(new GoalModule(
        //                            new List<GM_Precondition>(),
        //                            new List<Goal>() {
        //                                new Goal(new StateInventoryStatic(townies[bob].townieInformation.id, "strawberry_cake", 1, 100), 4)
        //                            }
        //                        ));

        /*townies[clara].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateSocial("clara", "alicia", Relationship.RelationType.friendly, -100, -5), 1)
                                    }
                                ));*/
        townies[clara].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[clara].townieInformation.id, "strawberry", 20, 100), 1)
                                    }
                                ));
        townies[dirk].gm.AddModule(new GoalModule(
                                    new List<GM_Precondition>(),
                                    new List<Goal>() {
                                        new Goal(new StateInventoryStatic(townies[dirk].townieInformation.id, "tulip", 20, 100), 1)
                                    }
                                ));

        townies[alicia].ws.knownFacts.AddFact(new WorldFactResource("river_farm", "common_fish", "trout"), townies[alicia].ws);
        townies[alicia].ws.knownFacts.AddFact(new WorldFactResource("river_feild", "common_fish", "trout"), townies[alicia].ws);
        townies[clara].ws.knownFacts.AddFact(new WorldFactResource("brush_forest", "rare_forage", "strawberry"), townies[clara].ws);
        townies[dirk].ws.knownFacts.AddFact(new WorldFactResource("meadow_feild", "common_forage", "tulip"), townies[dirk].ws);

        return townies;
    }
}


public class ResourceCatagories
{
    public static string r_connectedLocation = "connectedLocation";

    public static string r_initiator = "initiator";
    public static string r_recipient = "recipient";

}