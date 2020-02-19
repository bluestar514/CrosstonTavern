using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PeopleDetailTab : DetailTab
{

    public Person displayedPerson;

    public void Set(Person person)
    {
        displayedPerson = person;

        displayName.text = person.Id;
        location.text = person.location;


        body.text = person.StringifyStats();
    }



}
