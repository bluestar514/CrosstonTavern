using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using System.Linq;
using System;

public class LoadWorldData
{
    JSON totalJsonLocation;
    JSON totalJsonFeature;
    JSON totalJsonPeople;
    JSON totalJsonAction;

    public LoadWorldData(TextAsset actionJsonFile,TextAsset locationsJsonFile, TextAsset featuresJsonFile, TextAsset peopleJsonFile)
    {
        totalJsonAction = JSON.ParseString(actionJsonFile.text);
        totalJsonAction.DebugInEditor("Actions");

        totalJsonLocation = JSON.ParseString(locationsJsonFile.text);
        totalJsonLocation.DebugInEditor("Locations");

        totalJsonFeature = JSON.ParseString(featuresJsonFile.text);
        totalJsonFeature.DebugInEditor("Features");

        totalJsonPeople = JSON.ParseString(peopleJsonFile.text);
        totalJsonPeople.DebugInEditor("People");

    }

    public WorldState BuildWorld()
    {
        Dictionary<string, GenericAction> actions = InitializeActions();
        List<Location> locations = InitializeLocations();
        List<Feature> features = InitializeFeatures(actions, locations);

        List<Person> people = InitializePeople();

        AddPeopleToFeatures(features, people, actions);

        Map map = new Map(features, locations);
        Registry reg = new Registry(people);
        WorldTime worldTime = WorldTime.DayZeroEightAM;

        WorldState ws = new WorldState(map, reg, worldTime);

        SetRelations(ws.registry);
        SetGoals(ws, people);
        

        return ws;
    }

    protected Dictionary<string, GenericAction> InitializeActions()
    {
        Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>();

        foreach(JSON jsonAction in totalJsonAction.GetJArray("actions").AsJSONArray()) {
            string id = jsonAction.GetString("id");
            int time = jsonAction.GetInt("time");

            

            List<Condition> preconditions = new List<Condition>() {
                new Condition_SpaceAtFeature()
            };
            foreach (JSON jsonCondition in jsonAction.GetJArray("conditions").AsJSONArray()) {
                string type = jsonCondition.GetString("type");
                switch (type) {
                    //case "inv":
                    //    preconditions.Add(new Condition_IsState(ParseEffect(jsonCondition)));
                    //    break;
                    default:
                        Debug.LogWarning("Action (" + id + ") condition is of unrecognized type (" + type + ")");
                        break;
                }
            }

            List<Outcome> outcomes = new List<Outcome>();
            foreach (JSON jsonOutcome in jsonAction.GetJArray("outcomes").AsJSONArray()) {
                JSON jsonChance = jsonOutcome.GetJSON("chance");

                ChanceModifier chance = null;
                string type = jsonChance.GetString("type");
                switch (type) {
                    case "item":
                        string item = jsonChance.GetString("item");
                        string person = jsonChance.GetString("person");
                        string[] range = ParseRange(jsonChance.GetJArray("range"));
                        float minValue = float.Parse(range[0]);
                        float maxValue = float.Parse(range[1]);
                        chance = new ChanceModifierItemOpinion(item, person, minValue, maxValue);
                        break;
                    //case "relation":
                    //    int boundry = jsonChance.GetJNumber("boundry").AsInt();
                    //    EffectSocialChange socialState = (EffectSocialChange)ParseEffect(jsonChance, "social");
                    //    bool positive = jsonChance.GetBool("positive");

                    //    chance = new ChanceModifierRelation(socialState, boundry, positive);
                    //    break;
                    case "simple":
                        float value = jsonChance.GetJNumber("value").AsFloat();
                        chance = new ChanceModifierSimple(value);
                        break;
                    case "base":
                        chance = new ChanceModifier();
                        break;
                    default:
                        Debug.LogWarning("Action (" + id + ") chanceModifier type is of unrecognized type (" + type + ")");
                        break;
                }

                List<Effect> effects = new List<Effect>();
                foreach (JSON jsonEffect in jsonOutcome.GetJArray("effects").AsJSONArray()) {
                    effects.Add(ParseEffect(jsonEffect));
                }

                outcomes.Add(new Outcome(chance, effects));
            }

            GenericAction action = new GenericAction(id, time, preconditions, outcomes, null);
            actions.Add(id, action);
            
        }

        return actions;
    }

    protected List<Location> InitializeLocations()
    {
        Dictionary<string, HashSet<string>> connections = new Dictionary<string, HashSet<string>>();


        foreach (JArray connection in totalJsonLocation.GetJArray("connections").AsJArrayArray()) {
            string[] con = connection.AsStringArray();
            string a = con[0];
            string b = con[1];

            if (connections.ContainsKey(a)) {
                connections[a].Add(b);
            } else {
                connections.Add(a, new HashSet<string>() { b });
            }

            if (connections.ContainsKey(b)) {
                connections[b].Add(a);
            } else {
                connections.Add(b, new HashSet<string>() { a });
            }
        }

        List<Location> locations = new List<Location>();
        foreach(JSON location in totalJsonLocation.GetJArray("locations").AsJSONArray()) {
            string id = location.GetString("id");

            JSON jsonResources = location.GetJSON("resources");
            StringStringListDictionary resources = ParseResources(jsonResources);

            resources.Add(ResourceCatagories.r_connectedLocation, new List<string>(connections[id]));

            locations.Add(new Location(id, resources));
        }


        return locations;
    }

    protected List<Feature> InitializeFeatures(Dictionary<string, GenericAction> actions, List<Location> locations)
    {
        List<Feature> features = new List<Feature>();

        foreach(JSON feature in totalJsonFeature.GetJArray("features").AsJSONArray()) {
            string id = feature.GetString("id");
            string location = feature.GetString("location");
            int maxUsers = feature.GetInt("maxUsers");

            List<GenericAction> genericActions = new List<GenericAction>();
            foreach (string actionId in feature.GetJArray("actions").AsStringArray()) {
                if (actions.ContainsKey(actionId)) {
                    genericActions.Add(actions[actionId]);
                } else {
                    Debug.LogWarning("Action (" + actionId + ") not recognized. Will not be added to Feature (" + id + ")");
                }                
            }

            JSON jsonResources = feature.GetJSON("resources");
            StringStringListDictionary resources = ParseResources(jsonResources);


            Dictionary<string, int> stockTable = new Dictionary<string, int>();
            if (feature.ContainsKey("stock")) {
                JSON jsonStock = feature.GetJSON("stock");
                stockTable = ParseStockTable(jsonStock);
            }
            

            features.Add(new Feature(id, location, maxUsers, genericActions, resources, stockTable));
        }


        AddDoors(features, actions, locations);

        return features;
    }

    protected List<Person> InitializePeople()
    {
        List<Person> people = new List<Person>();

        foreach(JSON person in totalJsonPeople.GetJArray("people").AsJSONArray()) {
            string id = person.GetString("id");
            string initialLocation = person.GetString("location");

            JSON job = person.GetJSON("job");
            int[] start = job.GetJArray("shiftStart").AsIntArray();
            int[] end = job.GetJArray("shiftEnd").AsIntArray();

            EmploymentData employment = new EmploymentData() {
                title = job.GetString("title"),
                establishment = job.GetString("location"),
                shiftStart = new WorldTime(-1, -1, -1, start[0], start[1]),
                shiftEnd = new WorldTime(-1, -1, -1, end[0], end[1])
            };

            JSON jsonInv = person.GetJSON("inventory");
            Inventory inventory= ParseInventory(jsonInv, id);

            Person townie = new Person(id);
            townie.employmentData = employment;
            townie.inventory = inventory;


            foreach(JSON familyMember in person.GetJArray("family").AsJSONArray()) {
                string memberName = familyMember.GetString("id");
                string relation = familyMember.GetString("relation");

                townie.family.Add(new FamilyData() { id = memberName, relation = relation });
            }


            people.Add(townie);
        }

        return people;
    }

    protected void SetGoals(WorldState ws, List<Person> people)
    {
        foreach (Person person in people) {
            person.gm = new GoalManager(ws, person);

            string establishment = person.employmentData.establishment;
            GoalModule profession = new GoalModule(
                new List<GM_Precondition>() {
                    new GM_Precondition_Time(person.employmentData.shiftStart, person.employmentData.shiftEnd)
                },
                new List<Goal>() {
                    new Goal(new EffectMovement(person.Id, ws.map.GetFeature(establishment).location), 5, 1 )
                });
            profession.name = "profession";

            List<string> stock = ws.map.GetFeature(establishment).relevantResources["stock"];
            List<Goal> goals = new List<Goal>();
            //foreach (string s in stock) {
            //    goals.Add(new Goal(new EffectInvChange(3, 1000, establishment, new List<string>() { s }), 3, 1));
            //}
            //goals.Add(new Goal(new EffectInvChange(10, 1000, establishment, stock), 2, 1));

            GoalModule restock = new GoalModule(new List<GM_Precondition>(), goals);
            restock.name = "restock";

            person.gm.AddModule(profession);
            person.gm.AddModule(restock);


            //foreach(FamilyData family in person.family) {
            //    GoalModule maintainFamily = new GoalModule(new List<GM_Precondition>(), new List<Goal>() {
            //        new Goal(new EffectSocialChange(5, 1000000, person.Id, family.id, Relationship.RelationType.friendly), 10, 1)
            //    });

            //    maintainFamily.name = "maintain family relation(" + family.id + ")";
            //    person.gm.AddModule(maintainFamily);
            //}


            //GoalModule notLonely = new GoalModule(
            //    new List<GM_Precondition>(),
            //    new List<Goal>() {
            //        new Goal(new State_HasRelations(3, Relationship.CodifiedRelationships.friends), 3, 1),
            //        new Goal(new State_HasRelations(1, Relationship.CodifiedRelationships.lovers), 2, 1),
            //        new Goal(new State_HasRelations(1, Relationship.CodifiedRelationships.enemies), 2, 1),
            //    }
            //);

            //notLonely.name = "have some interpersonal relations!";

            //person.gm.AddModule(notLonely);

        }
    }

    void SetRelations(Registry reg)
    {

        Dictionary<string, List<List<int>>> relationValues = new Dictionary<string, List<List<int>>>() {
            {"acquaintance", new List<List<int>>() {
                new List<int>(){-2, 2},
                new List<int>(){-4, 4}
            } },
            {"friend", new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){-4, 6}
            } },
            {"enemy", new List<List<int>>(){
                new List<int>(){-6, -2},
                new List<int>(){-6, 4}
            } },
            {"lover", new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){4, 10}
            } }
        };

        List<string> relations = new List<string>(relationValues.Keys);

        Dictionary<string, Dictionary<string, string>> relationMatrix = new Dictionary<string, Dictionary<string, string>>();

        foreach(Person personA in reg.GetPeople()) {
            string a = personA.Id;

            foreach(Person personB in reg.GetPeople()) {
                string b = personB.Id;

                if (!relationMatrix.ContainsKey(a)) relationMatrix[a] = new Dictionary<string, string>();
                if (!relationMatrix.ContainsKey(b)) relationMatrix[b] = new Dictionary<string, string>();

                if (relationMatrix[b].ContainsKey(a)) {
                    relationMatrix[a][b] = relationMatrix[b][a];
                    continue;
                }

                if (a == b) {
                    relationMatrix[a].Add(b, "self");
                    continue;
                }

                int num = Mathf.FloorToInt(UnityEngine.Random.value * relations.Count);

                relationMatrix[a].Add(b, relations[num]);
            }
        }


        foreach(Person person in reg.GetPeople()) {
            foreach(Person other in reg.GetPeople()) {
                string relation = relationMatrix[person.Id][other.Id];

                if (relation == "self") continue;
                else {
                    List<List<int>> valueRange = relationValues[relation];

                    int friendly = UnityEngine.Random.Range(valueRange[0][0], valueRange[0][1]);
                    int romance = UnityEngine.Random.Range(valueRange[1][0], valueRange[1][1]);

                    person.relationships.Increase(other.Id, Relationship.RelationType.friendly, friendly);
                    person.relationships.Increase(other.Id, Relationship.RelationType.romantic, romance);
                }
            }
        }
    }

    void AddDoors(List<Feature> features, Dictionary<string, GenericAction> actions, List<Location> locations)
    {
        foreach (Location location in locations) {
            List<Feature> doors = new List<Feature>(
                    from loc in location.resources["connectedLocation"]
                    select new Feature("door_" + location.Id + "->" + loc,
                                        location.Id, 10000,
                                        new List<GenericAction>() {
                                            actions["move"]
                                        },
                                        new Dictionary<string, List<string>>(){
                                            {Map.R_CONNECTEDLOCATION, new List<string>(){loc} }
                                        }
                                    )
                    );

            features.AddRange(doors);
        }
    }

    void AddPeopleToFeatures(List<Feature> features, List<Person> people, Dictionary<string, GenericAction> actions)
    {
        foreach (Person person in people) {
            person.location = "farm";
            person.feature.providedActions.Add(actions["talk"]);
            person.feature.providedActions.Add(actions["give_#inventory_item#"]);
            person.feature.location = person.location;

            features.Add(person.feature);
        }
    }

    Effect ParseEffect(JSON jsonEffect, string type="")
    {
        if(type =="")
            type = jsonEffect.GetString("type");


        string[] range = new string[2];
        if (type != "move") { 
            
            JArray jRange = jsonEffect.GetJArray("range");

            range = ParseRange(jRange);
        }

        //switch (type) {
        //    case "inv":
        //        string owner = jsonEffect.GetString("owner");
        //        List<string> items = new List<string>(jsonEffect.GetJArray("items").AsStringArray());

        //        return new EffectGenericInv(range[0], range[1], owner, items);
        //    case "social":

        //        string source = jsonEffect.GetString("source");
        //        string dest = jsonEffect.GetString("dest");
        //        Relationship.RelationType relType = (Relationship.RelationType)
        //                                            Enum.Parse(typeof(Relationship.RelationType), 
        //                                            jsonEffect.GetString("relType"));

        //        return new EffectSocialChange(int.Parse(range[0]), int.Parse(range[1]), source, dest, relType);

        //    case "move":
        //        return new EffectMove();
        //    default:
        //        throw new Exception("Effect with type " + type + " found. This is not a supported type.");

        return new Effect();
        //}      
    }

    string[] ParseRange(JArray jRange)
    {
        if (jRange.Length > 2) throw new Exception("Effect has more than two values specifying its range!");

        string[] range = new string[2];
        for (int i = 0; i < jRange.Length; i++) {
            if (jRange[i] is JString) {
                range[i] = ((JString)jRange[i]).AsString();
            }
            if (jRange[i] is JNumber) {
                range[i] = ((JNumber)jRange[i]).AsString();
            }
        }
        if (jRange.Length == 1) range[1] = range[0];

        return range;
    }

    StringStringListDictionary ParseResources(JSON jsonResources)
    {
        StringStringListDictionary resources = new StringStringListDictionary();

        foreach (string jsonResource in jsonResources.Keys) {
            List<string> list = new List<string>(jsonResources.GetJArray(jsonResource).AsStringArray());
            resources.Add(jsonResource, list);
        }

        return resources;
    }

    Inventory ParseInventory(JSON jsonInv, string id)
    {
        Inventory inv = new Inventory(id);

        foreach (string jsonInvEntry in jsonInv.Keys) {
            inv.ChangeInventoryContents(jsonInv.GetInt(jsonInvEntry), jsonInvEntry);
        }

        return inv;
    }

    Dictionary<string, int> ParseStockTable(JSON jsonTable)
    {
        Dictionary<string, int> table = new Dictionary<string, int>();

        foreach(string jsonStock in jsonTable.Keys) {
            table.Add(jsonStock, jsonTable.GetInt(jsonStock));
        }

        return table;
    }
}
