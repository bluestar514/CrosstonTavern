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

        SetGoals(ws, people);

        return ws;
    }

    protected Dictionary<string, GenericAction> InitializeActions()
    {
        Dictionary<string, GenericAction> actions = new Dictionary<string, GenericAction>();

        Dictionary<string, int> costDict = new Dictionary<string, int>();
        JSON costs = totalJsonAction.GetJSON("cost");
        foreach(string key in costs.Keys) {
            costDict.Add(key, costs.GetInt(key));
        }

        foreach(JSON jsonAction in totalJsonAction.GetJArray("actions").AsJSONArray()) {
            string id = jsonAction.GetString("id");
            int time = jsonAction.GetInt("time");

            Dictionary<string, int> items = new Dictionary<string, int>();
            List<int> counts = new List<int>() { 1 };
            if (id.Contains("#item#")) {
                items = costDict;
            } else {
                items.Add("filler", 1);
            }

            if (id.Contains("#number#")) {
                counts = new List<int>() { 1, 5 };
            }

            foreach(int number in counts) {
                foreach(KeyValuePair<string, int> itemCost in items) {
                    string item = itemCost.Key;
                    int cost = itemCost.Value;

                    string specificId = id.Replace("#item#", item).Replace("#number#", number.ToString());

                    List<Condition> preconditions = new List<Condition>() {
                        new Condition_SpaceAtFeature()
                    };
                    foreach (JSON jsonCondition in jsonAction.GetJArray("conditions").AsJSONArray()) {
                        string type = jsonCondition.GetString("type");
                        switch (type) {
                            case "inv":
                                preconditions.Add(new Condition_IsState(ParseMicroEffect(jsonCondition, number, cost, new List<string>() { item })));
                                break;
                            default:
                                Debug.LogWarning("Action (" + specificId + ") condition is of unrecognized type (" + type + ")");
                                break;
                        }
                    }

                    List<Outcome> outcomes = new List<Outcome>();
                    foreach (JSON jsonOutcome in jsonAction.GetJArray("outcomes").AsJSONArray()) {
                        JSON jsonChance = jsonOutcome.GetJSON("chance");

                        ChanceModifier chance = null;
                        string type = jsonChance.GetString("type");
                        switch (type) {
                            case "simple":
                                float value = jsonChance.GetJNumber("value").AsFloat();
                                chance = new ChanceModifierSimple(value);
                                break;
                            case "base":
                                chance = new ChanceModifier();
                                break;
                            default:
                                Debug.LogWarning("Action (" + specificId + ") chanceModifier type is of unrecognized type (" + type + ")");
                                break;
                        }

                        List<Effect> effects = new List<Effect>();
                        foreach (JSON jsonEffect in jsonOutcome.GetJArray("effects").AsJSONArray()) {
                            effects.Add(ParseMicroEffect(jsonEffect, number, cost, new List<string>() { item }));
                        }

                        outcomes.Add(new Outcome(chance, effects));
                    }

                    GenericAction action = new GenericAction(specificId, time, preconditions, outcomes);
                    actions.Add(specificId, action);
                }

            }
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

            features.Add(new Feature(id, location, maxUsers, genericActions, resources));
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
                    new Goal(new Move(ws.map.GetFeature(establishment).location), 5, 1 )
                });
            profession.name = "profession";

            List<string> stock = ws.map.GetFeature(establishment).relevantResources["stock"];
            List<Goal> goals = new List<Goal>();
            foreach (string s in stock) {
                goals.Add(new Goal(new InvChange(3, 1000, establishment, new List<string>() { s }), 3, 1));
            }
            goals.Add(new Goal(new InvChange(10, 1000, establishment, stock), 2, 1));

            GoalModule restock = new GoalModule(new List<GM_Precondition>(), goals);
            restock.name = "restock";

            person.gm.AddModule(profession);
            person.gm.AddModule(restock);


            foreach(FamilyData family in person.family) {
                GoalModule maintainFamily = new GoalModule(new List<GM_Precondition>(), new List<Goal>() {
                    new Goal(new SocialChange(5, 1000000, person.Id, family.id, Relationship.RelationType.friendly), 1, 1)
                });

                maintainFamily.name = "maintain family relation(" + family.id + ")";
                person.gm.AddModule(maintainFamily);
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

    Effect ParseMicroEffect(JSON jsonMicroeffect, int number = 1, int cost = 1, List<string> items = null)
    {
        string type = jsonMicroeffect.GetString("type");

        switch (type) {
            case "inv":
                JArray jRange = jsonMicroeffect.GetJArray("range");
                int[] range = new int[2];
                for(int i=0; i< jRange.Length; i++) {

                    if (jRange[i] is JString) { 
                        JString str = (JString)jRange[i];
                        string result = str.AsString();
                        int value = 1;
                        if (result.Contains("-")) value *= -1;
                        if (result.Contains("#number#")) value *= number;
                        if (result.Contains("#cost#")) value *= cost;

                        range[i] = value;
                    }else { range[i] = jRange.GetInt(i); }

                }

                int min = range[0];
                int max = range[0];
                if (jRange.Length > 1) {
                    max = range[1];
                }

                string owner = "#" + jsonMicroeffect.GetString("owner") + "#";
                List<string> itemsMain = new List<string>(jsonMicroeffect.GetJArray("items").AsStringArray());
                if (itemsMain.Contains("#item#")) {
                    itemsMain.Remove("#item#");
                    itemsMain.AddRange(items);
                }

                return new InvChange(min, max, owner, itemsMain);
            case "social":
                range = jsonMicroeffect.GetJArray("range").AsIntArray();
                min = range[0];
                max = range[0];
                if (range.Length > 1) {
                    max = range[1];
                }

                string source = "#" + jsonMicroeffect.GetString("source") + "#";
                string dest = "#" + jsonMicroeffect.GetString("dest") + "#";

                string relType = jsonMicroeffect.GetString("relType");
                Relationship.RelationType rel = (Relationship.RelationType)Enum.Parse(typeof(Relationship.RelationType),relType);

                return new SocialChange(min, max, source, dest, rel);
            case "move":
                return new Move();
            default:
                Debug.LogWarning("MicroEffect of unrecognized type (" + type + ") found!");
                return null;
        }
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


}
