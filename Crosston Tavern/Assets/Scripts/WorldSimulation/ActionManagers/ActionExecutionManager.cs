using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Job is to determine the exact outcome of a Chosen Action.
/// Generates a Executed Action
/// </summary>

public class ActionExecutionManager : ActionManager
{
    Townie actor;
    WorldState ws;

    public ActionExecutionManager(Townie actor, WorldState ws)
    {
        this.actor = actor;
        this.ws = ws;
    }


    public ExecutedAction ExecuteAction(ChosenAction chosenAction)
    {

        Outcome chosenOutcome = ChooseOutcome(chosenAction.Action);

        List<Effect> realizedEffects = RealizeEffectsOfOutcome(chosenOutcome, chosenAction.Action.Bindings, 
                                                ws.map.GetFeature(chosenAction.Action.FeatureId).relevantResources);

        Outcome realizedOutcome = new Outcome(new ChanceModifierSimple(1), realizedEffects);

        return new ExecutedAction(chosenAction, realizedOutcome);

    }

    Outcome ChooseOutcome(WeightedAction action)
    {
        BoundBindingCollection bindings = action.Bindings;
        FeatureResources featureResources = ws.map.GetFeature(action.FeatureId).relevantResources;

        float totalChance = action.potentialOutcomes.Sum(outcome => outcome.chanceModifier.MakeBound(bindings, featureResources).Chance(ws));
        float rand = UnityEngine.Random.value * totalChance;

        foreach(Outcome outcome in action.potentialOutcomes) {
            float num = outcome.chanceModifier.MakeBound( bindings,featureResources).Chance(ws);

            if (num > rand) return outcome;
            rand -= num;
        }

        throw new System.Exception("Something was not programed correctly, we should not be making it to this line of code");
    }

    List<Effect> RealizeEffectsOfOutcome(Outcome chosenOutcome, BoundBindingCollection bindings, FeatureResources resources)
    {
        List<Effect> realizedEffects = new List<Effect>();

        foreach(Effect effect in chosenOutcome.effects) {
            realizedEffects.Add(effect.ExecuteEffect(ws, actor, bindings, resources));
        }

        return realizedEffects;
    }
}
