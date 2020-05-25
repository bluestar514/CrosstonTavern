using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleInitializer
{
    public static List<GenericAction> peopleActions = new List<GenericAction>() {
        ActionInitializer.actions["talk"],
        ActionInitializer.actions["give_#item#"],
        ActionInitializer.actions["insult"],
        ActionInitializer.actions["compliment"],
        ActionInitializer.actions["start_dating"]
    };

    public static Dictionary<string, Person> GetAllPeople() {
        Dictionary<string, Person> allPeople = new Dictionary<string, Person>() {
            {"organizer_alicia",    new Person("organizer_alicia",  "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"lover_bob",           new Person("lover_bob",         "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"vendor_clara",        new Person("vendor_clara",      "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"judge_dirk",          new Person("judge_dirk",        "farm", 2, peopleActions, new Dictionary<string, List<string>>())}//,
            //{"everet",  new Person("everet",    "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"faraz",   new Person("faraz",     "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"gigi",    new Person("gigi",      "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"henri",   new Person("henri",     "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"isabel",  new Person("isabel",    "farm", 2, peopleActions, new Dictionary<string, List<string>>())}
        };

        // Fill all values at random for everyone initially
        SetRelationsRandom(allPeople);
        SetInventoryRandom(allPeople);
        SetPreferencesRandom(allPeople);

        // Override with specific things for the senario and or testing
            //Lover scenario Details:
            allPeople["lover_bob"].relationships.Set("alicia", Relationship.RelationType.friendly, 2);
            allPeople["lover_bob"].relationships.Set("alicia", Relationship.RelationType.romantic, 5);

            allPeople["organizer_alicia"].relationships.Set("lover_bob", Relationship.RelationType.friendly, 3);
            allPeople["organizer_alicia"].relationships.Set("lover_bob", Relationship.RelationType.romantic, 0);


            allPeople["lover_bob"].inventory.ChangeInventoryContents(3, "trout");
            allPeople["lover_bob"].inventory.ChangeInventoryContents(2, "dragon_egg");
            allPeople["organizer_alicia"].inventory.ChangeInventoryContents(1, "fishing_rod");

            allPeople["organizer_alicia"].preferences[PreferenceLevel.loved].Add("dragon_egg");

            //Organizer scenario details:




        return allPeople;
    }

    static void SetRelationsRandom(Dictionary<string, Person> allPeople)
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

    static List<string> randomItems = new List<string>() {
            "apple", "banana", "trout", "ice_cream", "firewood", "coal", "morning_glory", "rose", "dandilion",
            "mushroom", "saphire", "gold_ore", "cabbage", "pancakes", "pizza", "french_fries"
        };

    static void SetInventoryRandom(Dictionary<string, Person> allPeople)
    {
        foreach(Person person in allPeople.Values) {
            person.inventory.ChangeInventoryContents(Random.Range(10, 25), "currency");
            List<string> items = chooseSeveralFrom(randomItems, Random.Range(randomItems.Count / 5, randomItems.Count / 2))[true];
            foreach(string item in items) {
                person.inventory.ChangeInventoryContents(Random.Range(1, 10), item);
            }
        }
    }

    static void SetPreferencesRandom(Dictionary<string, Person> allPeople)
    {
        foreach(Person person in allPeople.Values) {
            List<string> possibleItems = new List<string>(randomItems);

            person.preferences[PreferenceLevel.hated].Add("currency");

            foreach (PreferenceLevel level in System.Enum.GetValues(typeof(PreferenceLevel))) {
                Dictionary<bool, List<string>> randomSet = chooseSeveralFrom(possibleItems, Random.Range(1, randomItems.Count / 5));

                person.preferences[level].AddRange(randomSet[true]);
                possibleItems = randomSet[false];
            }

        }
    }

    static Dictionary<bool, List<string>> chooseSeveralFrom(List<string> list, int count)
    {
        List<string> options = new List<string>(list);
        List<string> chosen = new List<string>();
        for(; count > 0; count--) {
            int rand = Random.Range(0, options.Count);

            chosen.Add(options[rand]);
            options.RemoveAt(rand);
        }
        
        return new Dictionary<bool, List<string>>() {
            {true, chosen },
            {false, options }
        };
    }
}
