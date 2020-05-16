using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowniePanel : MonoBehaviour
{
    public string townie;
    [System.NonSerialized]
    public Person person;
    public Text display;

    public PeopleDetailTab PeopleDetailTab;

    public void Set(Person person, PeopleDetailTab PeopleDetailTab)
    {
        this.person = person;
        this.townie = person.id;
        this.PeopleDetailTab = PeopleDetailTab;

        display.text = this.townie;
    }

    public void OpenPeopleDetailTab()
    {
        PeopleDetailTab.gameObject.SetActive(true);
        PeopleDetailTab.Set(person);
    }
}
