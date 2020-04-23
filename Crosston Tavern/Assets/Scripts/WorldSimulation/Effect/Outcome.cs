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

    public List<Effect> BindEffects(Dictionary<string, List<string>> resources)
    {
        return new List<Effect>(from effect in effects
                                     select effect.BindEffect(resources));        
    }

    public float EvaluateChance()
    {
        evaluatedChance = chanceModifier.Chance();
        return evaluatedChance;
    }

    public override string ToString()
    {
        return "{"+ string.Join(",", effects)+"}";
    }
}
