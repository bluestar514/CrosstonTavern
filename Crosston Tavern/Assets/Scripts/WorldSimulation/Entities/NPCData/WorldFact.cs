using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WorldFact
{
    [SerializeField]
    protected string id = "{WorldFact:Generic}";

    public virtual WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        return this;
    }

    public virtual void UpdateWorldState(WorldState ws) { }
}


public class WorldFactResource: WorldFact { 

    public string featureId;
    public string resourceId;
    public string potentialBinding;

    public WorldFactResource(string featureId, string resourceId, string potentialBinding)
    {
        this.featureId = featureId;
        this.resourceId = resourceId;
        this.potentialBinding = potentialBinding;

        id = ToString();
    }

    public override string ToString()
    {
        return "{" + featureId + ":(" + resourceId + "," + potentialBinding + ")}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactResource)) return false;

        WorldFactResource fact = (WorldFactResource)obj;
        if (this.featureId == fact.featureId &&
            this.resourceId == fact.resourceId &&
            this.potentialBinding == fact.potentialBinding) return true;

        return false;
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string featureId = bindings.BindString(this.featureId);
        string potentialBinding = bindings.BindString(this.potentialBinding);
        
        return new WorldFactResource(featureId, this.resourceId, potentialBinding);
    }

    public override void UpdateWorldState(WorldState ws)
    {
        Feature feature = ws.map.GetFeature(featureId);

        feature.relevantResources.Add(resourceId, potentialBinding);
    }
}


public class WorldFactPreference: WorldFact
{
    public string person;
    public PreferenceLevel level;
    public string item;

    public WorldFactPreference(string person, PreferenceLevel level, string item)
    {
        this.person = person;
        this.level = level;
        this.item = item;

        id = ToString();
    }

    public override string ToString()
    {
        return "{" + person + ":(" + level.ToString() + "," + item + ")}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactPreference)) return false;

        WorldFactPreference fact = (WorldFactPreference)obj;
        if (this.person == fact.person &&
            this.level == fact.level &&
            this.item == fact.item) return true;

        return false;
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string person = bindings.BindString(this.person);
        string item = bindings.BindString(this.item);

        return new WorldFactPreference(person, this.level, item);
    }

    public override void UpdateWorldState(WorldState ws)
    {
        PreferencesDictionary pref = ws.registry.GetPerson(person).preferences;

        if (!pref.ContainsKey(level)) pref.Add(level, new List<string>());
        if (!pref[level].Contains(item)) pref[level].Add(item);
    }
}