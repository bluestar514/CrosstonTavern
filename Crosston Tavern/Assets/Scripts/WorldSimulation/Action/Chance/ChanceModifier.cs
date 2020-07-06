using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChanceModifier 
{

    /// <summary>
    /// </summary>
    /// <param name="ws"></param>
    /// <returns>Should return a value between 0 and 1, roughly representing the percent chance of an outcome occuring</returns>
    public virtual float Chance(WorldState ws)
    {
        return 1;
    }

    public virtual ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        return new ChanceModifier();
    }

    public virtual List<Goal> MakeGoal(WorldState ws, float priority)
    {
        return null;
    }
}

public class ChanceModifierSimple: ChanceModifier
{
    float chance;

    public ChanceModifierSimple(float chance)
    {
        this.chance = chance;
    }

    public override float Chance(WorldState ws)
    {
        return chance;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        return new ChanceModifierSimple(chance);
    }

     
}


