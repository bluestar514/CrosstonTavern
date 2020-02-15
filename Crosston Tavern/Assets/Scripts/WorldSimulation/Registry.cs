﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Registry 
{
    [SerializeField] List<Person> People;

    Dictionary<string, Person> people;

    public Registry(List<Person> people)
    {
        People = people;

        this.people = new Dictionary<string, Person>();
        foreach (Person person in People) {
            this.people.Add(person.Id, person);
        }
    }


    public Person GetPerson(string id)
    {
        if (people.ContainsKey(id)) return people[id];
        return null;
    }

    public IEnumerable<Person> GetPeople()
    {
        return from person in people
               select person.Value;
    }
}
