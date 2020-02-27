using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition 
{
    [SerializeField]
    protected string name = "GenericCondition";
    public virtual bool InEffect(Person actor, Feature feature, Location location)
    {
        return true;
    }
}

public class ConditionNotYou : Condition
{
    public ConditionNotYou()
    {
        name = "Condition:Don'tTargetSelf";
    }
    public override bool InEffect(Person actor, Feature feature, Location location)
    { 
        return actor.feature.Id != feature.Id;
    }
}

public class ConditionSpaceAtFeature: Condition
{
    public ConditionSpaceAtFeature()
    {
        name = "Condition:Can'tTargetFullFeature";
    }
    public override bool InEffect(Person actor, Feature feature, Location location)
    {
        return feature.currentUsers < feature.maxUsers;
    }
}