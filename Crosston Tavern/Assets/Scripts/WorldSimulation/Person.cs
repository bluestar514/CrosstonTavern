using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }
    public string location;
    public Feature feature;
    

    public Person(string id)
    {
        Id = id;
    }
}
