using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectKnowledge : Effect
{
    public WorldFact fact;

    public EffectKnowledge(WorldFact fact)
    {
        this.fact = fact;
    }


    /// <summary>
    /// For Knowledge Effect, Execution is done when the effect is processed by adding the executed effect in history,
    /// Instead the effect is bond to the best of our abilities
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="townie"></param>
    /// <param name="bindings"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        return new EffectKnowledge(fact.Bind(bindings, resources));
    }

    public override string ToString()
    {
        return "<EffectKnowledge(" + fact + ")>";
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        //TO DO!!!!

        return base.WeighAgainstGoal(ws, bindings, resources, goal);
    }
}
