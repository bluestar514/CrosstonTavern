using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierCombination : ChanceModifier
{
    List<ChanceModifier> chanceModifiers;
    Mode mode;

    public enum Mode
    {
        additive,
        multiplicitive
    }

    public ChanceModifierCombination(List<ChanceModifier> chanceModifiers, Mode mode)
    {
        this.chanceModifiers = chanceModifiers;
        this.mode = mode;
    }

    public override float Chance(WorldState ws)
    {
        float chance = 0;

        switch (mode) {
            case Mode.multiplicitive:
                chance += 1;
                foreach (ChanceModifier chanceModifier in chanceModifiers) {
                    chance *= chanceModifier.Chance(ws);
                }
                break;
            case Mode.additive:
                foreach (ChanceModifier chanceModifier in chanceModifiers) {
                    chance += chanceModifier.Chance(ws);
                }
                chance /= chanceModifiers.Count;

                break;
        }
        return chance;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        List<ChanceModifier> list = new List<ChanceModifier>();
        foreach(ChanceModifier chance in chanceModifiers) {
            list.Add(chance.MakeBound(bindings, featureResources));
        }

        return new ChanceModifierCombination(list, mode);
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
