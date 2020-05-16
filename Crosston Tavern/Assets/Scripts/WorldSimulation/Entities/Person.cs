using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person: Feature
{
    public Relationship relationships;
    public StringStringListDictionary preferences;

    public List<Goal> knownGoals;

    public Person(string id, string location, int maxUsers,
            List<GenericAction> providedActions, Dictionary<string, List<string>> relevantResources,
            StringStringListDictionary preferences,
            Dictionary<string, int> stockTable = null) :
    base(id, location, maxUsers, providedActions, relevantResources, stockTable)
    {
        knownGoals = new List<Goal>();
        this.relationships = new Relationship();
        this.preferences = preferences;
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

