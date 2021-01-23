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
        ActionInitializer.actions["start_dating"],
        ActionInitializer.actions["ask_#item#"]
    };

    public static Dictionary<string, Person> GetAllPeople() {
        Dictionary<string, Person> allPeople = new Dictionary<string, Person>() {
            {"barkeep",   new Person("barkeep",     "SYSTEM", 2, peopleActions, new Dictionary<string, List<string>>())}, 
            {"alicia",    new Person("alicia",      "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"bob",       new Person("bob",         "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"clara",     new Person("clara",       "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            {"dirk",      new Person("dirk",        "blacksmith", 2, peopleActions, new Dictionary<string, List<string>>())}//,
            //{"everet",  new Person("everet",    "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"faraz",   new Person("faraz",     "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"gigi",    new Person("gigi",      "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"henri",   new Person("henri",     "farm", 2, peopleActions, new Dictionary<string, List<string>>())},
            //{"isabel",  new Person("isabel",    "farm", 2, peopleActions, new Dictionary<string, List<string>>())}
        };

        // Fill all values at random for everyone initially
        SetRelationsRandom(allPeople);
        //SetInventoryRandom(allPeople);
        

        // Override with specific things for the senario and or testing
            //Lover scenario Details:
            allPeople["bob"].relationships.Set("alicia", Relationship.RelationType.friendly, 2);
            allPeople["bob"].relationships.Set("alicia", Relationship.RelationType.romantic, 5);

            allPeople["alicia"].relationships.Set("bob", Relationship.RelationType.friendly, 3);
            allPeople["alicia"].relationships.Set("bob", Relationship.RelationType.romantic, 0);


        //allPeople["lover_bob"].inventory.ChangeInventoryContents(3, "trout");
        //allPeople["lover_bob"].inventory.ChangeInventoryContents(2, "dragon_egg");

            allPeople["alicia"].preference.Add("strawberry_cake",   PreferenceLevel.loved);
            allPeople["alicia"].preference.Add("salmon_fried",      PreferenceLevel.loved);
            allPeople["alicia"].preference.Add("morning_rose",      PreferenceLevel.loved);
            allPeople["alicia"].preference.Add("strawberry",        PreferenceLevel.liked);
            allPeople["alicia"].preference.Add("salmon",            PreferenceLevel.liked);
            allPeople["alicia"].preference.Add("rose",              PreferenceLevel.liked);
            allPeople["alicia"].preference.Add("dandilion",         PreferenceLevel.liked);
            allPeople["alicia"].preference.Add("blackberry",        PreferenceLevel.disliked);
            allPeople["alicia"].preference.Add("trout",             PreferenceLevel.disliked);
            allPeople["alicia"].preference.Add("tulip",             PreferenceLevel.disliked);
            allPeople["alicia"].preference.Add("blackberry_tart",   PreferenceLevel.hated);
            allPeople["alicia"].preference.Add("trout_stew",        PreferenceLevel.hated);
            allPeople["alicia"].preference.Add("evening_tulip",     PreferenceLevel.hated);


            allPeople["dirk"].inventory.ChangeInventoryContents(1, "strawberry_cake_recipe");


        SetPreferencesRandom(allPeople);


        //Clara Testing Rivals
        allPeople["clara"].relationships.Set("bob", Relationship.RelationType.friendly, 3);
        allPeople["bob"].relationships.Set("clara", Relationship.RelationType.friendly, 3);


        return allPeople;
    }

    static void SetRelationsRandom(Dictionary<string, Person> allPeople)
    {

        Dictionary<Relationship.RelationshipTag, List<List<int>>> relationValues = new Dictionary<Relationship.RelationshipTag, List<List<int>>>() {
            {Relationship.RelationshipTag.acquantences, new List<List<int>>() {
                new List<int>(){-2, 2},
                new List<int>(){-4, 4}
            } },
            {Relationship.RelationshipTag.friends, new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){-4, 6}
            } },
            {Relationship.RelationshipTag.enemies, new List<List<int>>(){
                new List<int>(){-6, -2},
                new List<int>(){-6, 4}
            } },
            {Relationship.RelationshipTag.lovers, new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){4, 10}
            } }
        };

        List<Relationship.RelationshipTag> relations = new List<Relationship.RelationshipTag>(relationValues.Keys);

        Dictionary<string, Dictionary<string, Relationship.RelationshipTag>> relationMatrix = new Dictionary<string, Dictionary<string, Relationship.RelationshipTag>>();

        foreach (Person personA in allPeople.Values) {
            string a = personA.id;

            foreach (Person personB in allPeople.Values) {
                string b = personB.id;
                
                if (!relationMatrix.ContainsKey(a)) relationMatrix[a] = new Dictionary<string, Relationship.RelationshipTag>();
                if (!relationMatrix.ContainsKey(b)) relationMatrix[b] = new Dictionary<string, Relationship.RelationshipTag>();

                if (relationMatrix[b].ContainsKey(a)) {
                    relationMatrix[a][b] = relationMatrix[b][a];
                    continue;
                }

                if (a == b) {
                    relationMatrix[a].Add(b, Relationship.RelationshipTag.self);
                    continue;
                }

                int num = Mathf.FloorToInt(UnityEngine.Random.value * relations.Count);

                relationMatrix[a].Add(b, relations[num]);
            }
        }


        foreach (Person person in allPeople.Values) {
            foreach (Person other in allPeople.Values) {
                Relationship.RelationshipTag relation = relationMatrix[person.id][other.id];

                if (relation == Relationship.RelationshipTag.self) continue;
                else {
                    List<List<int>> valueRange = relationValues[relation];

                    int friendly = UnityEngine.Random.Range(valueRange[0][0], valueRange[0][1]);
                    int romance = UnityEngine.Random.Range(valueRange[1][0], valueRange[1][1]);

                    person.relationships.Increase(other.id, Relationship.RelationType.friendly, friendly);
                    person.relationships.Increase(other.id, Relationship.RelationType.romantic, romance);

                    person.relationships.AddRelationTag(other.id, relation);
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

            person.preference.Add("currency", PreferenceLevel.hated);

            foreach (PreferenceLevel level in System.Enum.GetValues(typeof(PreferenceLevel))) {
                Dictionary<bool, List<string>> randomSet = chooseSeveralFrom(possibleItems, Random.Range(1, randomItems.Count / 5));

                foreach(string item in randomSet[true]) {
                    if (level == PreferenceLevel.neutral) continue;
                    if(person.preference.GetLevel(item) == PreferenceLevel.neutral)
                        person.preference.Add(item, level);
                }
                
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
