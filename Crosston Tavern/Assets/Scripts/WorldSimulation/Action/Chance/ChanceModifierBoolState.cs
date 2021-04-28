using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierBoolState : ChanceModifier
{
    State state;
    bool match;

    public ChanceModifierBoolState(State state, bool match)
    {
        this.state = state;
        this.match = match;
    }

    public override float Chance(WorldState ws)
    {
        if (state.InEffect(ws, new BoundBindingCollection(), new FeatureResources())) {
            if (match) return 1;
            else return 0;
        } else {
            if (match) return 0;
            else return 1;
        }
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        return new ChanceModifierBoolState(state.Bind(bindings, featureResources), match);
    }
}