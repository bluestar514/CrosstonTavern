using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person: Feature
{
    public Relationship relationships;
    public PreferencesDictionary preferences;

    public List<Goal> knownGoals;
    public List<TimeObligation> timeObligations;

    public Person(string id, string location, int maxUsers,
            List<GenericAction> providedActions, Dictionary<string, List<string>> relevantResources,
            Dictionary<string, int> stockTable = null) :
    base(id, location, maxUsers, providedActions, relevantResources, stockTable)
    {
        knownGoals = new List<Goal>();
        this.relationships = new Relationship();
        this.preferences = new PreferencesDictionary() { };
        foreach(PreferenceLevel level in Enum.GetValues(typeof(PreferenceLevel))) {
            preferences.Add(level, new List<string>());
        }
    }

    public Person(Feature f): base(f.id, f.location, f.maxUsers, f.providedActions, f.relevantResources.resources, f.stockTable)
    {
        knownGoals = new List<Goal>();
        this.relationships = new Relationship();
        this.preferences = new PreferencesDictionary() { };
        foreach (PreferenceLevel level in Enum.GetValues(typeof(PreferenceLevel))) {
            preferences.Add(level, new List<string>());
        }
    }

    public override Feature Copy(bool perfect)
    {
        Person p = new Person(base.Copy(perfect));


        p.preferences = new PreferencesDictionary();
        if (perfect) {
            p.knownGoals = new List<Goal>(knownGoals);
            p.preferences.CopyFrom(preferences);
        } else {
            p.knownGoals = new List<Goal>();
            foreach (PreferenceLevel level in Enum.GetValues(typeof(PreferenceLevel))) {
                p.preferences.Add(level, new List<string>());
            }
        }

        p.relationships = relationships.Copy(perfect);


        return p;
    }


    public bool NeedItem(string item)
    {

        foreach (Goal g in knownGoals) {
            if (g.state is StateInventory) {
                StateInventory state = (StateInventory)g.state;
                if (state.itemId.Contains(item)) return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return "<"+id+">";
    }
}

[Serializable]
public enum PreferenceLevel
{
    hated,
    disliked,
    neutral, 
    liked, 
    loved
}