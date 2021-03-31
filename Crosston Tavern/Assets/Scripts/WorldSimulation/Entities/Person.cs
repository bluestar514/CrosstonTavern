using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person: Feature
{
    public Relationship relationships;
    public ItemPreference preference;

    public string profession;

    public List<Goal> knownGoals;
    public Schedule schedule;

    public Person(string id, string location, int maxUsers,
            List<GenericAction> providedActions, Dictionary<string, List<string>> relevantResources, string profession,
            Dictionary<string, int> stockTable = null) :
    base(id, FeatureType.person, location, maxUsers, providedActions, relevantResources, stockTable)
    {
        knownGoals = new List<Goal>();
        this.relationships = new Relationship();

        preference = new ItemPreference();
        this.profession = profession;

        schedule = new Schedule();
    }

    public Person(Feature f): base(f.id, FeatureType.person, f.location, f.maxUsers, f.providedActions, f.relevantResources.resources, f.stockTable)
    {
        knownGoals = new List<Goal>();
        this.relationships = new Relationship();
        preference = new ItemPreference();

        schedule = new Schedule();
    }

    public override Feature Copy(bool perfect)
    {
        Person p = new Person(base.Copy(perfect));
        

        if (perfect) {
            p.knownGoals = new List<Goal>(knownGoals);
            p.schedule = schedule.Copy();
        } else {
            p.knownGoals = new List<Goal>();
            
        }

        p.preference = preference.Copy(perfect);
        
        p.relationships = relationships.Copy(perfect);
        p.profession = profession;


        return p;
    }


    public bool NeedItem(string item)
    {

        foreach (Goal g in knownGoals) {
            if ( g is GoalState goalState && 
                goalState.state is StateInventory state &&
                state.itemId.Contains(item)) return true;
        }

        return false;
    }
    public PreferenceLevel ItemPreference(string item)
    {
        return preference.GetLevel(item);
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