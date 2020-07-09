using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PeopleDetailTab : DetailTab
{
    public Text location;
    public Text body;
    public MemoryAdder memoryAdder;

    public WorldHub worldHub;

    [System.NonSerialized]
    public Person displayedPerson;

    public void Set(Person person)
    {
        displayedPerson = person;

        displayName.text = person.id;
        location.text = person.location;


        Townie townie = new List<Townie>(from t in worldHub.allPeople
                                         where t.townieInformation == person
                                         select t)[0];

        memoryAdder.Setup(worldHub.ws, townie);

        //body.text = person.StringifyStats();
    }



}
