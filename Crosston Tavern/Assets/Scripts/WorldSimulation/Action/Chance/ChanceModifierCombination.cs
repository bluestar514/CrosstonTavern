using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierCombination : ChanceModifier
{
    List<ChanceModifier> chanceModifiers;

    public ChanceModifierCombination(List<ChanceModifier> chanceModifiers)
    {
        this.chanceModifiers = chanceModifiers;
    }

    public override float Chance(WorldState ws)
    {
        float chance = 1;
        foreach(ChanceModifier chanceModifier in chanceModifiers) {
            chance *= chanceModifier.Chance(ws);
        }

        return chance;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        List<ChanceModifier> list = new List<ChanceModifier>();
        foreach(ChanceModifier chance in chanceModifiers) {
            list.Add(chance.MakeBound(bindings, featureResources));
        }

        return new ChanceModifierCombination(list);
    }

    public override List<Goal> MakeGoal(WorldState ws, float priority)
    {
        List<Goal> goals = new List<Goal>();

        foreach (ChanceModifier chance in chanceModifiers) {
            List<Goal> result = chance.MakeGoal(ws, priority);
            if(result != null) goals.AddRange(result);
        }

        return goals;
    }
}
