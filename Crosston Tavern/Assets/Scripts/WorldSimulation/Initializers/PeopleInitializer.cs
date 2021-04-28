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
            {"profession", new List<string>(){ "farmer" } },
            {"loves", new List<string>(){"stewed_trout"} },
            {"likes", new List<string>(){"blackberry", "trout", "blackberry_tart", "strawberry_cake"} },
            {"dislikes", new List<string>(){"carrot", "bell_pepper"} },
            {"hates", new List<string>(){"carrot_cake"} },
            {"inventory", new List<string>(){} }
        },
        new Dictionary<string, List<string>>() {
            {"name", new List<string>(){"sammy"}},
            {"home", new List<string>(){"sammysHouse"} },
            {"profession", new List<string>(){ "fisher" } },
            {"loves", new List<string>(){"strawberry_cake"} },
            {"likes", new List<string>(){"strawberry", "salmon", "fried_salmon"} },
            {"dislikes", new List<string>(){"blackberry", "trout"} },
            {"hates", new List<string>(){"blackberry_tart"} },
            {"inventory", new List<string>(){} }
        },
        new Dictionary<string, List<string>>() {
            {"name", new List<string>(){"finley"}},
            {"home", new List<string>(){"finleysHouse"} },
            {"profession", new List<string>(){ "forager" } },
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

    static Dictionary<string, Dictionary<string, Dictionary<Relationship.Axis, relationPair>>> initialRelations =
        new Dictionary<string, Dictionary<string, Dictionary<Relationship.Axis, relationPair>>>() {
            {"avery", new Dictionary<string, Dictionary<Relationship.Axis, relationPair>>(){
                //has a crush on sammy
                    {"sammy", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(20, 15) },
                        {Relationship.Axis.romantic, new relationPair(40, 10) }  }
                    },
                // good friends with finely
                    {"finley", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(40, 45) },
                        {Relationship.Axis.romantic, new relationPair(5, 5) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5, 5) },
                        {Relationship.Axis.romantic, new relationPair(0,0) }  }
                    }
                }
            },
            {"sammy", new Dictionary<string, Dictionary<Relationship.Axis, relationPair>>(){
                // thinks favorably of avery, but isn't in love with them at all
                    {"avery", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(20, 25) },
                        {Relationship.Axis.romantic, new relationPair(-5, 5) }  }
                    },
                //Has a crush on finely, but doesn't think they like them at all
                    {"finley", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(10, -15) },
                        {Relationship.Axis.romantic, new relationPair(30, -5) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5, 5) },
                        {Relationship.Axis.romantic, new relationPair(0, 0) }  }
                    }
                }
            },
            {"finley", new Dictionary<string, Dictionary<Relationship.Axis, relationPair>>(){
                // good friends with Avery and has a secret crush they think is completely not reciprocated
                    {"avery", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(40, 45) },
                        {Relationship.Axis.romantic, new relationPair(20, -10) }  }
                    },
                // really doesn't like sammy at all
                    {"sammy", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(-20, -20) },
                        {Relationship.Axis.romantic, new relationPair(0, 0) }  }
                    },
                    {"barkeep", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5, 5) },
                        {Relationship.Axis.romantic, new relationPair(0, 0) }  }
                    }
                }
            },
            {"barkeep", new Dictionary<string, Dictionary<Relationship.Axis, relationPair>>(){
                    {"avery", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5, 5) },
                        {Relationship.Axis.romantic, new relationPair(0,0) }  }
                    },
                    {"sammy", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5,5) },
                        {Relationship.Axis.romantic, new relationPair(0,0) }  }
                    },
                    {"finley", new Dictionary<Relationship.Axis, relationPair>() {
                        {Relationship.Axis.friendly, new relationPair(5,5) },
                        {Relationship.Axis.romantic, new relationPair(0,0) }  }
                    }
                }
            }
    };

    public static Dictionary<string, Person> GetAllPeople() {

        Dictionary<string, Person> allPeople = new Dictionary<string, Person>() {
            {"barkeep",   new Person("barkeep",     "SYSTEM", 2, peopleActions, new Dictionary<string, List<string>>(), "barkeep")}, 
        };

        foreach(Dictionary<string, List<string>> personData in peopleData) {
            Person person = new Person(personData["name"][0], 
                                        personData["home"][0], 
                                        2, 
                                        peopleActions, 
                                        new Dictionary<string, List<string>>(),
                                        personData["profession"][0]);

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

            //TODO: Things added to a character's initial inventory don't seem to actually get added. 
            //Should track this down when I get a chance
            foreach(string item in personData["inventory"]) {
                person.inventory.ChangeInventoryContents(1, item);
            }

            allPeople.Add(personData["name"][0], person);
        }

        foreach(string name in initialRelations.Keys) {
            Person person = allPeople[name];

            foreach(string other in initialRelations[name].Keys) {
                foreach (Relationship.Axis axis in initialRelations[name][other].Keys) {
                    person.relationships.Set(other, axis, initialRelations[name][other][axis].to);
                }
            }
        }

        return allPeople;
    }


    public static void SetAssumedPerceptionsOfOthers(Townie townie)
    {

        foreach(KeyValuePair <string, Dictionary <Relationship.Axis, relationPair>> relation in initialRelations[townie.Id]) {
            string other = relation.Key;
            Dictionary<Relationship.Axis, relationPair> initialRelationData = relation.Value;

            Relationship liveRelationship = townie.ws.GetRelationshipsFor(other);

            foreach (KeyValuePair<Relationship.Axis, relationPair> pair in initialRelationData) {
                Relationship.Axis axis = pair.Key;
                int initialValue = pair.Value.from;

                liveRelationship.Set(townie.Id, axis, initialValue);
            }
        }
        
    }
    
    public static void SetInitialRelationTags(Townie townie)
    {
        foreach(Person person in townie.ws.map.GetPeople()) {
            Relationship rel = person.relationships;

            foreach (string other in rel.GetKnownPeople()) {
                foreach (KeyValuePair<Relationship.Tag,
                    Dictionary<Relationship.Axis, float[]>> pair in Relationship.codifiedRelationRanges) {
                    Relationship.Tag tag = pair.Key;
                    Dictionary<Relationship.Axis, float[]> data = pair.Value;

                    bool inRange = true;
                    foreach (Relationship.Axis axis in data.Keys) {
                        int currentValue = person.relationships.Get(other, axis);

                        if (currentValue < data[axis][0] ||
                            currentValue > data[axis][1]) {
                            inRange = false;
                            break;
                        }
                    }

                    if (inRange) {
                        rel.AddRelationTag(other, tag);
                    }

                }

            }
        }
    }



    ///DEPRECIATED:


    static void SetRelationsRandom(Dictionary<string, Person> allPeople)
    {

        Dictionary<Relationship.Tag, List<List<int>>> relationValues = new Dictionary<Relationship.Tag, List<List<int>>>() {
            {Relationship.Tag.acquantences, new List<List<int>>() {
                new List<int>(){-2, 2},
                new List<int>(){-4, 4}
            } },
            {Relationship.Tag.friend, new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){-4, 6}
            } },
            {Relationship.Tag.enemy, new List<List<int>>(){
                new List<int>(){-6, -2},
                new List<int>(){-6, 4}
            } },
            {Relationship.Tag.in_love_with, new List<List<int>>(){
                new List<int>(){2, 6},
                new List<int>(){4, 10}
            } }
        };

        List<Relationship.Tag> relations = new List<Relationship.Tag>(relationValues.Keys);

        Dictionary<string, Dictionary<string, Relationship.Tag>> relationMatrix = new Dictionary<string, Dictionary<string, Relationship.Tag>>();

        foreach (Person personA in allPeople.Values) {
            string a = personA.id;

            foreach (Person personB in allPeople.Values) {
                string b = personB.id;
                
                if (!relationMatrix.ContainsKey(a)) relationMatrix[a] = new Dictionary<string, Relationship.Tag>();
                if (!relationMatrix.ContainsKey(b)) relationMatrix[b] = new Dictionary<string, Relationship.Tag>();

                if (relationMatrix[b].ContainsKey(a)) {
                    relationMatrix[a][b] = relationMatrix[b][a];
                    continue;
                }

                if (a == b) {
                    relationMatrix[a].Add(b, Relationship.Tag.self);
                    continue;
                }

                int num = Mathf.FloorToInt(UnityEngine.Random.value * relations.Count);

                relationMatrix[a].Add(b, relations[num]);
            }
        }


        foreach (Person person in allPeople.Values) {
            foreach (Person other in allPeople.Values) {
                Relationship.Tag relation = relationMatrix[person.id][other.id];

                if (relation == Relationship.Tag.self) continue;
                else {
                    List<List<int>> valueRange = relationValues[relation];

                    int friendly = UnityEngine.Random.Range(valueRange[0][0], valueRange[0][1]);
                    int romance = UnityEngine.Random.Range(valueRange[1][0], valueRange[1][1]);

                    person.relationships.Increase(other.id, Relationship.Axis.friendly, friendly);
                    person.relationships.Increase(other.id, Relationship.Axis.romantic, romance);

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
