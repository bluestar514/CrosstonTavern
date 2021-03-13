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


    public virtual List<State> Combine(State state)
    {
        return new List<State>() { this, state };
    }

    public virtual string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense = true)
    {
        return id;
    }

    public override bool Equals(object obj)
    {
        if (obj is State state) {
            return id == state.id;

        } else return false;
    }

    public override int GetHashCode()
    {
        return 1877310944 + EqualityComparer<string>.Default.GetHashCode(id);
    }
}
