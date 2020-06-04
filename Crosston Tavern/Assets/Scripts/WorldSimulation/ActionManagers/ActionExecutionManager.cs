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
    List<Townie> townies;

    public ActionExecutionManager(Townie actor, WorldState ws, List<Townie> townies)
    {
        this.actor = actor;
        this.ws = ws;
        this.townies = townies;
    }


    public ExecutedAction ExecuteAction(ChosenAction chosenAction)
    {
        string location = actor.townieInformation.location;

        Outcome chosenOutcome = ChooseOutcome(chosenAction.Action);

        FeatureResources resources = ws.map.GetFeature(chosenAction.Action.FeatureId).relevantResources;
        BoundBindingCollection bindings = chosenAction.Action.Bindings;
        bindings.BindResources(resources);

        List <Effect> realizedEffects = RealizeEffectsOfOutcome(chosenOutcome, bindings, resources);

        Outcome realizedOutcome = new Outcome(new ChanceModifierSimple(1), realizedEffects);

        ExecutedAction executedAction = new ExecutedAction(chosenAction, realizedOutcome, ws.Time);

        RealizeActionForTownies(location, executedAction);
        if (location != actor.townieInformation.location)
            RealizeActionForTownies(actor.townieInformation.location, executedAction);

        return executedAction;

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

        foreach (Effect effect in chosenOutcome.effects) {
            realizedEffects.Add(effect.ExecuteEffect(ws, null, bindings, resources));
        }

        return realizedEffects;
    }


    void RealizeActionForTownies(string location, ExecutedAction executedAction)
    {
        WeightedAction action = executedAction.Action;

        List <Effect> realizedEffects = action.potentialOutcomes[0].effects;
        BoundBindingCollection bindings = action.Bindings;
        FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;

        foreach (Townie townie in townies) {
            if(townie.townieInformation.location == location) {


                foreach (Effect effect in realizedEffects) {
                    effect.ExecuteEffect(townie.ws, townie, bindings, resources);
                }

                townie.history.Add(executedAction);
                townie.ws.AddHistory(executedAction);
            }
        }
    }
}
