using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Outcome 
{
    public ChanceModifier chanceModifier;
    public List<Effect> effects;

    public float evaluatedChance;

    public Outcome(ChanceModifier chanceModifier, List<Effect> effects)
    {
        this.chanceModifier = chanceModifier;
        this.effects = effects;
    }

    public float EvaluateChance(WorldState ws, BoundBindingCollection bindings, FeatureResources featureResources)
    {
        evaluatedChance = chanceModifier.MakeBound(bindings, featureResources).Chance(ws);
        return evaluatedChance;
    }

    public override string ToString()
    {
        return "{"+ string.Join(",", effects)+"}";
    }
}
