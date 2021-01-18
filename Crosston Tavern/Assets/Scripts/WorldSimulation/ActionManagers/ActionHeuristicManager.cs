using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Two Jobs:
/// 1. Create Weights for all potential Bound Actions (ie, make the Weighted Actions)
/// 2. Pick the "Best" next action, given those weighted Choices (Generate a single Chosen Action)
/// </summary>

public class ActionHeuristicManager : ActionManager
{
    WorldState ws;
    Person actor;

    public ActionHeuristicManager(Person actor, WorldState ws)
    {
        this.ws = ws;
        this.actor = actor;
    }

    public List<WeightedAction> WeighActions(List<BoundAction> boundActions = null, bool onlyValid = true)
    {
        if(boundActions == null) {
            ActionBuilder ab = new ActionBuilder(ws, actor);
            boundActions = ab.GetAllActions();
        }

        List<BoundAction> possibleActions = boundActions;

        if (onlyValid) {
            Dictionary<bool, List<BoundAction>> validActions = GetPossibleActions(possibleActions);
            possibleActions = validActions[true];
        }

        List<WeightedAction> weightedActions = new List<WeightedAction>();
        foreach(BoundAction action in possibleActions) {
            weightedActions.Add(GetWeightOfBoundAction(action));
        }

        return weightedActions;

    }

    public ChosenAction ChooseBestAction(List<WeightedAction> weightedActions = null, List<BoundAction> rejectedActions=null)
    {
        if(weightedActions == null) {
            weightedActions = WeighActions();
        }
        if(rejectedActions == null) {
            rejectedActions = GetPossibleActions(new ActionBuilder(ws, actor).GetAllActions())[false];
        }

        weightedActions = new List<WeightedAction>( weightedActions.OrderByDescending(a => a.weight));

        //Debug.Log(string.Join(",", weightedActions));

        int maxConsideration = Mathf.Min(5, weightedActions.Count);

        float totalWeight = 0;
        for(int i = 0; i< maxConsideration; i++) {
            if (weightedActions[i].weight <= 0) continue;
            totalWeight += weightedActions[i].weight;
        }

        float rand = UnityEngine.Random.Range(0, totalWeight);
        WeightedAction action = weightedActions[0];
        for(int i=0; i< maxConsideration; i++) {
            if (rand < weightedActions[i].weight) {

                action = weightedActions[i];
                break;
            } else rand -= weightedActions[i].weight;
        }

        weightedActions.Remove(action);

        return new ChosenAction(action, rejectedActions, weightedActions);
    }

    public Dictionary<bool, List<BoundAction>> GetPossibleActions(List<BoundAction> boundActions)
    {
        List<BoundAction> possibleActions = new List<BoundAction>();
        List<BoundAction> rejectedActions = new List<BoundAction>();

        foreach (BoundAction action in boundActions) {
            FeatureResources resources = ws.map.GetFeature(action.FeatureId).relevantResources;

            if (action.preconditions.Valid(ws, actor, action.Bindings,resources)) possibleActions.Add(action);
            else rejectedActions.Add(action);
        }

        return new Dictionary<bool, List<BoundAction>>() {
            {true, possibleActions },
            {false, rejectedActions }
        };
    }

    public WeightedAction GetWeightOfBoundAction(BoundAction boundAction, List<Goal> goals = null)
    {
        //Weight = sum of(chance of occuring * desirablity of outcome)


        List<WeightedAction.WeightRational> rationals = new List<WeightedAction.WeightRational>();
        float weight = 0;
        foreach(Outcome outcome in boundAction.potentialOutcomes) {
            BoundBindingCollection bindings = boundAction.Bindings;
            FeatureResources resources = ws.map.GetFeature(boundAction.FeatureId).relevantResources;

            KeyValuePair < float, List<WeightedAction.WeightRational> > result = 
                                                            GetWeightOfOutcome(outcome, bindings, resources, goals);

            weight += result.Key;
            rationals.AddRange(result.Value);
        }

        return new WeightedAction(boundAction, weight, rationals);
    }

    public KeyValuePair<float, List<WeightedAction.WeightRational>> 
        GetWeightOfOutcome(Outcome outcome, BoundBindingCollection bindings, FeatureResources resources, List<Goal> goals=null)
    {
        if (goals == null) {
            goals = actor.knownGoals;
        }

        float chance = outcome.EvaluateChance(ws, bindings, resources);
        float weight = 0;
        List<WeightedAction.WeightRational> rationals = new List<WeightedAction.WeightRational>();

        foreach (Goal goal in goals) {
            foreach (Effect effect in outcome.effects) {
                float desirability = GetWeightOfEffectGivenGoal(effect, bindings, resources, goal) ;
                float value = desirability * chance;
                weight += value;
                if (value != 0)
                    rationals.Add(new WeightedAction.WeightRational(effect, goal, value, desirability, chance));
            }
        }
        return new KeyValuePair<float, List<WeightedAction.WeightRational>>(weight, rationals) ;
    }

    float GetWeightOfEffectGivenGoal(Effect effect, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        return effect.WeighAgainstGoal(ws, bindings, resources, goal) * goal.priority;
    }

}