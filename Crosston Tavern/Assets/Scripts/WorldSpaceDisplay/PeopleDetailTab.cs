﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PeopleDetailTab : DetailTab
{
    public Text location;

    public WorldHub worldHub;

    public Transform goalsHolder;
    public Transform itemsHolder;
    public Transform relationsHolder;

    public Transform otherHolder;

    public GameObject entryPrefab;

    [System.NonSerialized]
    public Townie displayedTownie;
    Person displayedPerson;

    public void Set(Townie person)
    {
        displayedTownie = person;
        displayedPerson = person.townieInformation;

        displayName.text = person.Id;
        location.text = displayedPerson.location;

        Clear();
        Fill();
    }

    void Clear()
    {
        foreach (Transform parent in new List<Transform>() { goalsHolder, itemsHolder, relationsHolder, otherHolder }) {
            foreach (Transform child in parent) {
                if(child.GetComponent<DisplayEntry>() != null)
                    Destroy(child.gameObject);
            }
        }
    }

    void Fill()
    {

        Inventory inv = displayedPerson.inventory;
        foreach (string item in inv.GetItemList()) {
            Instantiate(entryPrefab, itemsHolder)
                .GetComponent<DisplayEntry>().Init(item + ":" + inv.GetInventoryCount(item));
        }

        foreach(Goal goal in displayedPerson.knownGoals) {
            Instantiate(entryPrefab, goalsHolder)
                .GetComponent<DisplayEntry>().Init(goal.ToString());
        }

        Relationship rel = displayedPerson.relationships;
        foreach (string people in rel.GetKnownPeople()) {
            IEnumerable<string> relTags = from tag in rel.GetTag(people)
                                          select tag.ToString();

            Relationship reverseRel = displayedTownie.ws.GetRelationshipsFor(people);

            Instantiate(entryPrefab, relationsHolder)
                .GetComponent<DisplayEntry>().Init(people + ":("+ 
                                                    rel.Get(people, Relationship.Axis.friendly) +","+
                                                    rel.Get(people, Relationship.Axis.romantic) + ") - ("+
                                                    reverseRel.Get(displayedTownie.Id, Relationship.Axis.friendly)+","+
                                                    reverseRel.Get(displayedTownie.Id, Relationship.Axis.romantic) + ") - {"+
                                                    string.Join(", ", relTags)+"}");
        }


        foreach(Person other in displayedTownie.ws.map.GetPeople()) {
            Instantiate(entryPrefab, otherHolder)
                .GetComponent<DisplayEntry>().Init(other.id + ":(" + other.preference + ")");
        }
    }


}
