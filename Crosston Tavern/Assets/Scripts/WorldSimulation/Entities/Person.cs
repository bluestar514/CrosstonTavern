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


    public bool NeedItem(string item)
    {

        //foreach(Goal g in knownGoals) {
        //    if(g.state is EffectInvChange) {
        //        EffectInvChange state = (EffectInvChange)g.state;
        //        if (state.ItemId.Contains(item)) return true;
        //    }
        //}

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