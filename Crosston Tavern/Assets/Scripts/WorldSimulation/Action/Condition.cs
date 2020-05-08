using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition 
{
    [SerializeField]
    protected string name = "GenericCondition";
    public virtual bool InEffect(Person actor, Feature feature, Location location, WorldState ws)
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
    public Condition_NotYou()
    {
        name = "Condition:Don'tTargetSelf";
    }
    public override bool InEffect(Person actor, Feature feature, Location location, WorldState ws)
    { 
        return actor.feature.Id != feature.Id;
    }
}

public class Condition_SpaceAtFeature: Condition
{
    public Condition_SpaceAtFeature()
    {
        name = "Condition:Can'tTargetFullFeature";
    }
    public override bool InEffect(Person actor, Feature feature, Location location, WorldState ws)
    {
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

    public override bool InEffect(Person actor, Feature feature, Location location, WorldState ws)
    {
        return false;//state.GoalComplete(ws, actor);
    }
}