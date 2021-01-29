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

    public virtual List<WorldFact> UpdateWorldState(WorldState ws) {
        return new List<WorldFact>() { this };
    }


    public virtual string Verbalize(string speaker, string listener, WorldState ws=null)
    {
        return id;
    }
}


[System.Serializable]
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

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        Feature feature = ws.map.GetFeature(featureId);

        feature.relevantResources.Add(resourceId, potentialBinding);

        return base.UpdateWorldState(ws);
    }

    public override int GetHashCode()
    {
        var hashCode = 1424682090;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(featureId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(resourceId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(potentialBinding);
        return hashCode;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws=null)
    {
        return "There is " + potentialBinding + " at " + featureId;
    }
}


[System.Serializable]
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

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        ItemPreference pref = ws.registry.GetPerson(person).preference;

        pref.Add(item, level);

        return base.UpdateWorldState(ws);
    }

    public override int GetHashCode()
    {
        var hashCode = 1523044811;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(person);
        hashCode = hashCode * -1521134295 + level.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(item);
        return hashCode;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string level;
        string name = person;

        if (this.level == PreferenceLevel.neutral) {
            if (name == speaker || name == listener) level = "don't care one way or the other about";
            else level = "doesn't care one way or the other about";
        } else {
            level = this.level.ToString();
            level = level.Remove(level.Length - 1, 1);
            if (name != speaker && name != listener) level += "s";
        }

        if (name == speaker) name = "I";
        if (name == listener) name = "you";
        

        return name + " " + level + " " + item;
    }
}

[System.Serializable]
public class WorldFactEvent: WorldFact
{
    public ExecutedAction action;

    public WorldFactEvent(ExecutedAction action)
    {
        this.action = action;

        id = ToString();
    }

    public override string ToString()
    {
        return "{Event:" + action.ToString() + "}";
    }


    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactEvent)) return false;

        WorldFactEvent fact = (WorldFactEvent)obj;
        if (this.action.ToString() == fact.action.ToString()) return true;

        return false;
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        List<WorldFact> learnedFacts = ws.AddHistory(action);
        learnedFacts.Add(this);

        return learnedFacts;
    }

    public override int GetHashCode()
    {
        return -1387187753 + EqualityComparer<ExecutedAction>.Default.GetHashCode(action);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);
        return v.VerbalizeAction(action, false);
    }
}

[System.Serializable]
public class WorldFactGoal: WorldFact
{
    public Goal goal;
    public string owner;

    public WorldFactGoal(Goal goal, string owner)
    {
        this.goal = goal;

        id = ToString();
        this.owner = owner;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactGoal)) return false;
        WorldFactGoal other = (WorldFactGoal)obj;

        return goal == other.goal;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "{Goal:"+ owner+ " - " + goal.ToString()+"}";
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);
        
        return owner+ " wants " + v.VerbalizaeState(goal.state);
    }
}

[System.Serializable]
public class WorldFactPotentialAction : WorldFact
{
    public BoundAction action;

    public WorldFactPotentialAction(BoundAction action)
    {
        this.action = action;

        id = ToString();
    }

    public override string ToString()
    {
        return "{PotentialAction:" + action.ToString() + "}";
    }


    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactPotentialAction)) return false;

        WorldFactPotentialAction fact = (WorldFactPotentialAction)obj;
        if (this.action.ToString() == fact.ToString()) return true;

        return false;
    }


    public override int GetHashCode()
    {
        return -1387187753 + EqualityComparer<BoundAction>.Default.GetHashCode(action);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);
        return v.VerbalizeAction(action, true);
    }
}