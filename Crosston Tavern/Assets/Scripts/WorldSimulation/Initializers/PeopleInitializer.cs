using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleInitializer
{
    public static List<GenericAction> peopleActions = ActionInitializer.GetAllPeopleActions();

    //avery, sammy, finley

    static List<Dictionary<string, List<string>>> peopleData = new List<Dictionary<string, List<string>>>() {
        new Dictionary<string, List<string>>() {
            {"name", new List<string>(){"avery"}},
            {"home", new List<string>(){"averysHouse"} },
            {"loves", new List<string>(){"stewed_trout"} },
            {"likes", new List<string>(){"blackberry", "trout", "blackberry_tart", "strawberry_cake"} },
            {"dislikes", new List<string>(){"carrot", "bell_pepper"} },
            {"hates", new List<string>(){"carrot_cake"} },
            {"inventory", new List<string>(){} }
        },
        new Dictionary<string, List<string>>() {
            {"name", new List<string>(){"sammy"}},
            {"home", new List<string>(){"sammysHouse"} },
            {"loves", new List<string>(){"strawberry_cake"} },
            {"likes", new List<string>(){"strawberry", "salmon", "fried_salmon"} },
            {"dislikes", new List<string>(){"blackberry", "trout"} },
            {"hates", new List<string>(){"blackberry_trout"} },
            {"inventory", new List<string>(){} }
        },
        new Dictionary<string, List<string>>() {
            {"name", new List<string>(){"finley"}},
            {"home", new List<string>(){"finleysHouse"} },
            {"loves", new List<string>(){"blackberry_tart"} },
            {"likes", new List<string>(){"blackberry", "salmon", "fried_salmon"} },
            {"dislikes", new List<string>(){"bell_pepper"} },
            {"hates", new List<string>(){"veg_stirfry"} },
            {"inventory", new List<string>(){} }
        }
    };

    class relationPair
    {
        public int to; //what A thinks of B
        public int from; //what A thinks B thinks of A

        public relationPair(int to, int from)
        {
            this.to = to;
            this.from = from;
        }
    }

    static Dictionary<string, Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>> initialRelations =
        new Dictionary<string, Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>>() {
            {"avery", new Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>(){
                    {"sammy", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(20,15) },
                        {Relationship.RelationType.romantic, new relationPair(40, 10) }  }
                    },
                    {"finley", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(40, 45) },
                        {Relationship.RelationType.romantic, new relationPair(5, 0) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5, 5) },
                        {Relationship.RelationType.romantic, new relationPair(0,0) }  }
                    }
                }
            },
            {"sammy", new Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>(){
                    {"avery", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(20, 25) },
                        {Relationship.RelationType.romantic, new relationPair(-5, 5) }  }
                    },
                    {"finley", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(10, -15) },
                        {Relationship.RelationType.romantic, new relationPair(30, -5) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5, 5) },
                        {Relationship.RelationType.romantic, new relationPair(0, 0) }  }
                    }
                }
            },
            {"finley", new Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>(){
                    {"avery", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(40, 45) },
                        {Relationship.RelationType.romantic, new relationPair(20, -10) }  }
                    },
                    {"sammy", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(-10, -10) },
                        {Relationship.RelationType.romantic, new relationPair(0, 0) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5, 5) },
                        {Relationship.RelationType.romantic, new relationPair(0, 0) }  }
                    }
                }
            },
            {"barkeep", new Dictionary<string, Dictionary<Relationship.RelationType, relationPair>>(){
                    {"avery", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5, 5) },
                        {Relationship.RelationType.romantic, new relationPair(0,0) }  }
                    },
                    {"sammy", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5,5) },
                        {Relationship.RelationType.romantic, new relationPair(0,0) }  }
                    },
                    {"finley", new Dictionary<Relationship.RelationType, relationPair>() {
                        {Relationship.RelationType.friendly, new relationPair(5,5) },
                        {Relationship.RelationType.romantic, new relationPair(0,0) }  }
                    }
                }
            }
    };

    public static Dictionary<string, Person> GetAllPeople() {

        Dictionary<string, Person> allPeople = new Dictionary<string, Person>() {
            {"barkeep",   new Person("barkeep",     "SYSTEM", 2, peopleActions, new Dictionary<string, List<string>>())}, 
        };

        foreach(Dictionary<string, List<string>> personData in peopleData) {
            Person person = new Person(personData["name"][0], personData["home"][0], 2, peopleActions, new Dictionary<string, List<string>>());
            foreach(KeyValuePair<string, PreferenceLevel> catagory in 
                new List<KeyValuePair<string, PreferenceLevel>> {
                    new KeyValuePair<string, PreferenceLevel>("loves", PreferenceLevel.loved),
                    new KeyValuePair<string, PreferenceLevel>("likes", PreferenceLevel.liked),
                    new KeyValuePair<string, PreferenceLevel>("dislikes", PreferenceLevel.disliked),
                    new KeyValuePair<string, PreferenceLevel>("hates", PreferenceLevel.hated) }) {
                foreach(string item in personData[catagory.Key]) {
                    person.preference.Add(item, catagory.Value);
                }
            }

            foreach(string item in personData["inventory"]) {
                person.inventory.ChangeInventoryContents(1, item);
            }

            allPeople.Add(personData["name"][0], person);
        }

        foreach(string name in initialRelations.Keys) {
            Person person = allPeople[name];

            foreach(string other in initialRelations[name].Keys) {
                foreach (Relationship.RelationType axis in initialRelations[name][other].Keys) {
                    person.relationships.Set(other, axis, initialRelations[name][other][axis].to);
                }
            }
        }

        return allPeople;
    }


    public static void SetAssumedPerceptionsOfOthers(Townie townie)
    {

        foreach(KeyValuePair <string, Dictionary <Relationship.RelationType, relationPair>> relation in initialRelations[townie.Id]) {
            string other = relation.Key;
            Dictionary<Relationship.RelationType, relationPair> initialRelationData = relation.Value;

            Relationship liveRelationship = townie.ws.GetRelationshipsFor(other);

            foreach (KeyValuePair<Relationship.RelationType, relationPair> pair in initialRelationData) {
                Relationship.RelationType axis = pair.Key;
                int initialValue = pair.Value.from;

                liveRelationship.Set(townie.Id, axis, initialValue);
            }
        }
        
    }




    ///DEPRECIATED:


    static void SetRelationsRandom(Dictionary<string, Person> allPeople)
    {

        Dictionary<Relationship.RelationshipTag, List<List<int>>> relationValues = new Dictionary<Relationship.RelationshipTag, List<List<int>>>() {
            {Relationship.RelationshipTag.acquantences, new List<List<int>>() {
                new List<int>(){-2, 2},
                new List<int>(){-4, 4}
            } },
            {Relationship.RelationshipTag.friend, new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){-4, 6}
            } },
            {Relationship.RelationshipTag.enemy, new List<List<int>>(){
                new List<int>(){-6, -2},
                new List<int>(){-6, 4}
            } },
            {Relationship.RelationshipTag.in_love_with, new List<List<int>>(){
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
