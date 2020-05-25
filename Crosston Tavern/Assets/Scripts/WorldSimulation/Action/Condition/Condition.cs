﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition 
{
    [SerializeField]
    protected string name = "GenericCondition";
    public virtual bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings, FeatureResources featureResources)
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

    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings, FeatureResources featureResources)
    { 
        return actor.id != bindings.BindString(featureId);
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
    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings, FeatureResources featureResources)
    {
        Feature feature = ws.map.GetFeature(bindings.BindString(featureId));

        return feature.currentUsers < feature.maxUsers;
    }
}

public class Condition_IsState: Condition
{
    public State state;
    public bool isTrue;
    public Condition_IsState(State state, bool isTrue = true) {
        this.state = state;
        this.isTrue = isTrue;


        name = "Condition:" + state.ToString()+ "-" + isTrue;
    }

    public override bool InEffect(Person actor, WorldState ws, BoundBindingCollection bindings, FeatureResources featureResources)
    {
        bool inEffect = state.InEffect(ws, bindings, featureResources);
        
        return inEffect == isTrue;
    }
}