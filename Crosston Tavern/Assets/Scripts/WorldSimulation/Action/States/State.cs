using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State 
{
    public string id;


    public virtual bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        return false;
    }


    public virtual State Bind(BoundBindingCollection bindings, FeatureResources resources) { return this; }
}
