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

    public List<WeightedAction> WeighActions(List<BoundAction> boundActions = null)
    {
        if(boundActions == null) {
            ActionBuilder ab = new ActionBuilder(ws, actor);
            boundActions = ab.GetAllActions();
        }

        Dictionary<bool, List<BoundAction>> validActions = GetPossibleActions(boundActions);

        List<BoundAction> possibleActions = validActions[true];

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

        weightedActions.OrderByDescending(a => a.weight);

        int maxConsideration = Mathf.Min(5, weightedActions.Count);

        float totalWeight = 0;
        for(int i = 0; i< maxConsideration; i++) {
            totalWeight += weightedActions[i].weight;
        }

        float rand = UnityEngine.Random.Range(0, totalWeight);
        WeightedAction action = weightedActions[0];
        for(int i=0; i< maxConsideration; i++) {
            if (rand < weightedActions[i].weight) action = weightedActions[i];
            else rand -= weightedActions[i].weight;
        }

        weightedActions.Remove(action);

        return new ChosenAction(action, rejectedActions, weightedActions);
    }

    public Dictionary<bool, List<BoundAction>> GetPossibleActions(List<BoundAction> boundActions)
    {
        List<BoundAction> possibleActions = new List<BoundAction>();
        List<BoundAction> rejectedActions = new List<BoundAction>();

        foreach (BoundAction action in boundActions) {
            if (action.preconditions.Valid(ws, actor, action.Bindings)) possibleActions.Add(action);
            else rejectedActions.Add(action);
        }

        return new Dictionary<bool, List<BoundAction>>() {
            {true, possibleActions },
            {false, rejectedActions }
        };
    }

    WeightedAction GetWeightOfBoundAction(BoundAction boundAction)
    {
        //Weight = sum of(chance of occuring * desirablity of outcome)

        List<WeightedAction.WeightRational> rationals = new List<WeightedAction.WeightRational>();
        float weight = 0;
        foreach(Outcome outcome in boundAction.potentialEffects) {
            KeyValuePair < float, List<WeightedAction.WeightRational> > result = 
                GetWeightOfOutcome(outcome, boundAction.Bindings, ws.map.GetFeature(boundAction.FeatureId).relevantResources);

            weight += result.Key;
            rationals.AddRange(result.Value);
        }

        return new WeightedAction(boundAction, weight, rationals);
    }

    KeyValuePair<float, List<WeightedAction.WeightRational>> 
        GetWeightOfOutcome(Outcome outcome, BoundBindingCollection bindings, FeatureResources resources)
    {
        float chance = outcome.EvaluateChance(ws);
        float weight = 0;
        List<WeightedAction.WeightRational> rationals = new List<WeightedAction.WeightRational>();
        foreach (Goal goal in actor.knownGoals) {
            foreach (Effect effect in outcome.effects) {
                float value = GetWeightOfEffectGivenGoal(effect, bindings, resources, goal) * chance;
                weight += value;
                if (value != 0)
                    rationals.Add(new WeightedAction.WeightRational(effect, goal.state, value));
            }
        }
        return new KeyValuePair<float, List<WeightedAction.WeightRational>>(weight, rationals) ;
    }

    float GetWeightOfEffectGivenGoal(Effect effect, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if(effect is EffectInventory) {
            return GetWeightOfInventoryEffect((EffectInventory)effect, bindings, resources, goal);
        }else if(effect is EffectSocial) {
            return GetWeightOfSocialEffect((EffectSocial)effect, bindings, goal);
        }else if(effect is EffectMovement) {
            return GetWeightOfMovementEffect((EffectMovement)effect, bindings, resources, goal);
        } else {
            throw new Exception("Effect (" + effect + ") of unplanned for type. Cannot determine desirable weight.");
        }
    }

    float GetWeightOfInventoryEffect(EffectInventory effect, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (!(goal.state is StateInventory)) return 0;
        StateInventory state = (StateInventory)goal.state;

        string goalInvOwner = state.ownerId;
        string goalItem = state.itemId;

        string owner = bindings.BindString(effect.ownerId);

        if (goalInvOwner != owner.Replace("person_", "")) return 0;
        Inventory inv = ws.GetInventory(owner);

        if (effect is EffectInventoryStatic) {
            EffectInventoryStatic invEffect = (EffectInventoryStatic)effect;

            string item = bindings.BindString(invEffect.itemId);
            if (item != goalItem) return 0;
            int count = inv.GetInventoryCount(item);

            return CountInRange(count, invEffect.delta, state.min, state.max);
        } else if (effect is EffectInventoryVariable) {
            EffectInventoryVariable invVariable = (EffectInventoryVariable)effect;

            List<string> items = new List<string>();
            foreach(string item in from item in invVariable.itemIds
                                   select bindings.BindString(item)) {
                items.AddRange(resources.BindString(item));
            }

            if (!items.Contains(state.itemId)) return 0;
            int count = inv.GetInventoryCount(goalItem);

            return (CountInRange(count, invVariable.min, state.min, state.max) + 
                    CountInRange(count, invVariable.max, state.min, state.max)) 
                    / 2;
        } else {
            return 0;
        }
    }

    float GetWeightOfSocialEffect(EffectSocial effect, BoundBindingCollection bindings, Goal goal)
    {
        if (!(goal.state is StateSocial)) return 0;
        StateSocial state = (StateSocial)goal.state;


        string source = bindings.BindString(effect.sourceId);
        string target = bindings.BindString(effect.targetId);
        Relationship.RelationType axis = effect.axis;

        string goalSource = state.sourceId;
        string goalTarget = state.targetId;
        Relationship.RelationType goalAxis = state.axis;

        if (source != goalSource ||
            target != goalTarget ||
            axis != goalAxis) return 0;

        Relationship rel = ws.GetRelationshipsFor(source);
        int value = rel.Get(target, axis);

        if (effect is EffectSocialStatic) {
            EffectSocialStatic social = (EffectSocialStatic)effect;

            return CountInRange(value, social.delta, state.min, state.max);
        } else if (effect is EffectSocialVariable) {
            EffectSocialVariable social = (EffectSocialVariable)effect;

            return (CountInRange(value, social.min, state.min, state.max) +
                    CountInRange(value, social.max, state.min, state.max))
                    / 2;
        } else {
            return 0;
        }
    }

    float GetWeightOfMovementEffect(EffectMovement effect, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (!(goal.state is StatePosition)) return 0;
        StatePosition state = (StatePosition)goal.state;

        string mover = bindings.BindString(effect.moverId);
        string loc = bindings.BindString(effect.newLocationId);

        List<string> locations = resources.BindString(loc);

        Map map = ws.map;

        string currentLocation = ws.registry.GetPerson(mover).location;
        string goalLocation = state.locationId;

        if (mover != state.moverId) return 0;

        float weight = 0;

        foreach (string location in locations) {
            //get there in one step
            if (location == goalLocation) weight += 2;
            else weight -= 1;
            //Move in right direction
            if (map.GetDistance(currentLocation, goalLocation) > map.GetDistance(location, goalLocation))
                weight += 2 * (map.LocationCount - map.GetDistance(location, goalLocation)) / map.LocationCount;
            //Don't leave the goalLocation
            if (currentLocation == goalLocation) weight -= 1;
        }

        return weight/locations.Count;
    }

    float CountInRange(int count, int delta, int min, int max)
    {
        float weight = 0;

        int newCount = count + delta;

        //get there in one step:
        // in range -> +1
        // outside -> -1
        if (newCount <= max && newCount >= min) weight += 1;
        else weight -= 1;

        //less important if we are already in range
        // were in range -> -1
        if (count <= max && count >= min) weight -= 1;

        //move in the right direction:
        // if move in right direction from outside range -> +2
        // if move in wrong direction from outside range -> -2
        // if move from inside the range -> 0
        if (Mathf.Abs(newCount - min) <= Mathf.Abs(count - min)) weight += 1;
        else weight -= 1;
        if (Mathf.Abs(newCount - max) <= Mathf.Abs(count - max)) weight += 1;
        else weight -= 1;

        return weight;
    }

}