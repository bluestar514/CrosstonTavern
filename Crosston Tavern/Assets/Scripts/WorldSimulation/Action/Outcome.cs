using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Outcome 
{
    public ChanceModifier chanceModifier;
    public List<Effect> effects;
    public List<VerbilizationEffect> effectVerbalizations;

    public float evaluatedChance;

    public Outcome(ChanceModifier chanceModifier, List<Effect> effects, List<VerbilizationEffect> effectVerbalizations)
    {
        this.chanceModifier = chanceModifier;
        this.effects = effects;
        this.effectVerbalizations = effectVerbalizations;
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
