using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Registry 
{
    [SerializeField] string id;

    [SerializeField] List<Person> People;

    Dictionary<string, Person> people;

    public Registry(List<Person> people)
    {
        People = people;

        this.people = new Dictionary<string, Person>();
        foreach (Person person in People) {
            this.people.Add(person.id, person);
        }
    }

    public Registry(List<Feature> mapFeatures, string idTag = "copy")
    {
        List<Person> people = new List<Person>();
        foreach(Feature f in mapFeatures) {
            if(f is Person) {
                people.Add((Person)f);
            }
        }

        People = people;
        this.people = new Dictionary<string, Person>();
        foreach (Person person in People) {
            this.people.Add(person.id, person);
        }

        id = idTag; 
    }

    public Person GetPerson(string id)
    {
        id = id.Replace("person_", "");

        if (people.ContainsKey(id)) return people[id];
        return null;
    }

    public IEnumerable<Person> GetPeople()
    {
        return from person in people
               select person.Value;
    }
}
