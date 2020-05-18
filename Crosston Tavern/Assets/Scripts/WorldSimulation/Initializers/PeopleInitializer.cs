using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleInitializer
{
    public static List<GenericAction> peopleActions = new List<GenericAction>() {
        ActionInitializer.actions["talk"],
        ActionInitializer.actions["give_#item#"]
    };

    public static Dictionary<string, Person> GetAllPeople() {
        Dictionary<string, Person> allPeople = new Dictionary<string, Person>() {
            {"alicia",  new Person("alicia",    "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"bob",     new Person("bob",       "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"clara",   new Person("clara",     "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"dirk",    new Person("dirk",      "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"everet",  new Person("everet",    "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"faraz",   new Person("faraz",     "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"gigi",    new Person("gigi",      "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"henri",   new Person("henri",     "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())},
            {"isabel",  new Person("isabel",    "farm", 2, peopleActions, new Dictionary<string, List<string>>(), new StringStringListDictionary())}
        };

        SetRelations(allPeople);
        SetInventory(allPeople);
        

        allPeople["alicia"].inventory.ChangeInventoryContents(1, "fishing_rod");

        return allPeople;
    }

    static void SetRelations(Dictionary<string, Person> allPeople)
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

        foreach (Person personA in allPeople.Values) {
            string a = personA.id;

            foreach (Person personB in allPeople.Values) {
                string b = personB.id;
                
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


        foreach (Person person in allPeople.Values) {
            foreach (Person other in allPeople.Values) {
                string relation = relationMatrix[person.id][other.id];

                if (relation == "self") continue;
                else {
                    List<List<int>> valueRange = relationValues[relation];

                    int friendly = UnityEngine.Random.Range(valueRange[0][0], valueRange[0][1]);
                    int romance = UnityEngine.Random.Range(valueRange[1][0], valueRange[1][1]);

                    person.relationships.Increase(other.id, Relationship.RelationType.friendly, friendly);
                    person.relationships.Increase(other.id, Relationship.RelationType.romantic, romance);
                }
            }
        }
    }


    static void SetInventory(Dictionary<string, Person> allPeople)
    {
        foreach(Person person in allPeople.Values) {
            person.inventory.ChangeInventoryContents(Random.Range(1, 25), "currency");
        }
    }
}
