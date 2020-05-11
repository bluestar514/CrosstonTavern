using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition 
{
    [SerializeField]
    protected string name = "GenericCondition";
    public virtual bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings)
    {
        return true;
    }

    public override string ToString()
    {
        return name;
    }
}

public class Condition_NotYou : Condition
{
    public string featureId;

    public Condition_NotYou(string featureId)
    {
        this.featureId = featureId;

        name = "Condition:Don'tTargetSelf("+featureId+")";
    }

    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings)
    { 
        return actor.feature.Id != bindings.BindString(featureId);
    }
}

public class Condition_SpaceAtFeature: Condition
{

    public string featureId;
    public Condition_SpaceAtFeature(string featureId)
    {
        this.featureId = featureId;

        name = "Condition:Can'tTargetFullFeature("+featureId+")";
    }
    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings)
    {
        Feature feature = ws.map.GetFeature(bindings.BindString(featureId));

        return feature.currentUsers < feature.maxUsers;
    }
}

public class Condition_IsState: Condition
{
    public State state;

    public Condition_IsState(State state) {
        this.state = state;

        name = "Condition:" + state.ToString();
    }

    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings)
    {
        return false;//state.GoalComplete(ws, actor);
    }
}