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
    WorldState ws; //global world state
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

        ExecutedAction executedAction = new ExecutedAction(chosenAction, realizedEffects, chosenOutcome.effectVerbalizations, ws.Time);

        RealizeActionForTownies(location, executedAction);

        //exectute action in minds of townies at both destination and source
        if (location != actor.townieInformation.location)
            RealizeActionForTownies(actor.townieInformation.location, executedAction);

        ws.AddHistory(executedAction);

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

        // updates the global WS
        // which is why the Townies feild here is null because we arent' updating anyone's internal state
        foreach (Effect effect in chosenOutcome.effects) {
            realizedEffects.Add(effect.ExecuteEffect(ws, null, bindings, resources));
        }

        return realizedEffects;
    }


    void RealizeActionForTownies(string location, ExecutedAction executedAction)
    {
        WeightedAction action = executedAction.Action;

        List <Effect> realizedEffects = executedAction.executedEffect;
        BoundBindingCollection bindings = action.Bindings;
        FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;

        foreach (Townie townie in townies) {
            if(townie.townieInformation.location == location) {
                ExecutedAction personalAction = executedAction.ShallowCopy();
                AddOpinion(personalAction, townie);


                foreach (Effect effect in realizedEffects) {
                    effect.ExecuteEffect(townie.ws, townie, bindings, resources);
                }

                townie.ws.AddHistory(personalAction);
            }
        }
    }


    void AddOpinion(ExecutedAction executedAction, Townie townie)
    {
        Opinion opinion = new Opinion(executedAction, townie);

        executedAction.opinion = opinion;
    }


/*    List<Outcome> FilterOutcomesThatFullfillGoal(Goal goal, List<Outcome> outcomes, WeightedAction action)
    {
        List<Outcome> relevantOutcomes = new List<Outcome>();
        foreach (Outcome outcome in outcomes) {
            if (OutcomeProgressesGoal(outcome, goal, action)) relevantOutcomes.Add(outcome);
        }

        return relevantOutcomes;
    }

    bool OutcomeProgressesGoal(Outcome outcome, Goal goal, WeightedAction action)
    {
        BoundBindingCollection bindings = action.Bindings;
        FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;


        foreach (Effect effect in outcome.effects) {

            if (effect.WeighAgainstGoal(ws,bindings,resources, goal) > 0) {
                return true;
            }
        }

        return false;
    }*/
}
