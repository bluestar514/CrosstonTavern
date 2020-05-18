using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Precondition 
{
    public List<Condition> conditions;

    public Precondition(List<Condition> conditions)
    {
        this.conditions = conditions;
    }


    public bool Valid(WorldState ws, Person actor, BoundBindingCollection bindings, FeatureResources featureResources)
    {
        return conditions.TrueForAll(x => x.InEffect(actor, ws, bindings, featureResources));
    }
}
