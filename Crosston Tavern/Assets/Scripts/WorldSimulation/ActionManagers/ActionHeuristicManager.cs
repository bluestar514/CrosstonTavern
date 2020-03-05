using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionHeuristicManager : ActionManager
{
    Person actor;
    Registry people;
    Map map;
    WorldState WS;

    public ActionHeuristicManager(Person actor, WorldState ws)
    {
        this.actor = actor;
        WS = ws;
        people = WS.registry;
        map = WS.map;
    }

    //Public Facing:

    public List<WeightedAction> GenerateWeightedOptions(string location = "", int depth = 0, bool considerPreconditions = true)
    {
        if (depth >= 3) return new List<WeightedAction>();


        List<BoundAction> boundActions = GatherProvidedActionsFor(location);
        if(considerPreconditions) boundActions = FilterOnPrecondition(boundActions);
        List<WeightedAction> weightedActions = WeighOptions(boundActions, depth);

        return weightedActions;
    }

    public ChosenAction ChooseAction(List<WeightedAction> choices)
    {
        choices = new List<WeightedAction>(choices.OrderByDescending(action => action.weight));
        if (choices.Count == 0) return null;

        //int rand = Mathf.FloorToInt(UnityEngine.Random.value * 3);
        //rand = Mathf.Clamp(rand, 0, choices.Count-1);

        float totalWeight = 0;
        foreach(WeightedAction option in choices) {
            totalWeight += option.weight;
        }

        float rand = UnityEngine.Random.value * totalWeight;
        WeightedAction choice = choices[0];
        foreach (WeightedAction option in choices) {
            if (option.weight > rand) {
                choice = option;
                break;
            }
            rand -= option.weight;
        }

        choices.Remove(choice);

        return new ChosenAction(choice,GetRejectedOnPrecondition(GatherProvidedActionsFor()), choices);
    }

    //Steps toward Generating Weighted Options:


    public List<BoundAction> GatherProvidedActionsFor(string location="")
    {
        if (location == "") location = actor.location;
        return map.GatherProvidedActionsForActorAt(actor.Id, location);
    }

    List<BoundAction> FilterOnPrecondition(List<BoundAction> actions)
    {

        return new List<BoundAction>(from action in actions
                                     where action.SatisfiedPreconditions(WS)
                                     select action);

    }

    List<BoundAction> GetRejectedOnPrecondition(List<BoundAction> actions)
    {
        return new List<BoundAction>(from action in actions
                                     where !action.SatisfiedPreconditions(WS)
                                     select action);
    }

    List<WeightedAction> WeighOptions(List<BoundAction> actions, int depth)
    {
        List<WeightedAction> weightedActions = new List<WeightedAction>();

        foreach (BoundAction action in actions) {

            List<Effect> boundEffects = action.GenerateExpectedEffects(WS);

            weightedActions.Add(EvaluateAction(action, boundEffects, depth));
        }

        return weightedActions;
    }


    WeightedAction EvaluateAction(BoundAction action, List<Effect> boundEffects, int depth)
    {

        List<WeightedAction.WeightRational> weightRationals = new List<WeightedAction.WeightRational>();
        foreach (Effect effect in boundEffects) {
            float effectLikelyhood = effect.EvaluateChance();

            foreach (MicroEffect subeffect in effect.effects) {
                foreach (KeyValuePair<MicroEffect, float> kvp in actor.goalPriorityDict) {
                    MicroEffect goal = kvp.Key;
                    float priority = kvp.Value;

                    float weight = priority * effectLikelyhood * EvaluateEffectTowardGoal(subeffect, goal, depth);

                    weightRationals.Add(new WeightedAction.WeightRational(subeffect, goal, weight));
                }
            }
        }

        float total = 0;
        foreach (WeightedAction.WeightRational wr in weightRationals) {
            total += wr.weight;
        }

        return new WeightedAction(action, total, weightRationals, boundEffects);

    }

    public float EvaluateEffectTowardGoal(MicroEffect effect, MicroEffect goal, int depth)
    {

        goal = goal.BindEffect(actor.resources);

        if (goal is InvChange) {
            InvChange invGoal = (InvChange)goal;

            return EvaluateInvGoals(effect, invGoal, depth);
        }


        if (goal is Move) {
            Move locationGoal = (Move)goal;
            return EvaluateMoveGoals(effect, locationGoal);
        }


        if (goal is SocialChange) {
            SocialChange socialGoal = (SocialChange)goal;
            return 1;
        }

        return 0;
    }

    float EvaluateInvGoals(MicroEffect effect, InvChange goal, int depth)
    {

        if (effect is InvChange) {
            InvChange invChange = (InvChange)effect;

            //stop if the inventory this would add to is not the one we want
            if (invChange.InvOwner != goal.InvOwner) return 0;

            //stop if the item being changed isn't what we are looking for
            int matches = 0;
            int count = 0;
            foreach (string item in goal.ItemId) {
                if (invChange.ItemId.Contains(item)) matches++;
                if (actor.inventory.ContainsKey(item)) count += actor.inventory[item];
            }
            if (matches == 0) return 0;

            //Determine how much this helps us
            float DeltaAverage = (invChange.DeltaMin + invChange.DeltaMax) / 2;
            float weight = 0;
            if ((count < goal.DeltaMin && invChange.DeltaMin >= 0 && invChange.DeltaMax > 0)
                || (count > goal.DeltaMax && invChange.DeltaMax <= 0 && invChange.DeltaMin < 0))// moving in the right direction
                weight += Mathf.Abs(DeltaAverage);
            if (count + DeltaAverage <= goal.DeltaMax && count + DeltaAverage >= goal.DeltaMin)
                weight += Mathf.Abs(DeltaAverage);

            return weight;

        }

        if (effect is Move) {
            //Move move = (Move)effect;

            //List<WeightedAction> actionsInNewLocation = GenerateWeightedOptions(move.TargetLocation, depth+1);

            float weight = 0;
            //foreach (WeightedAction action in actionsInNewLocation) {
            //    weight += action.weight / (2 * actionsInNewLocation.Count);
            //}

            return weight;
        }

        if (effect is SocialChange) {
            //SocialChange socChange = (SocialChange)effect;

            return 0;
        }

        return 0;
    }

    float EvaluateMoveGoals(MicroEffect effect, Move goal)
    {
        if (effect is Move) {
            Move move = (Move)effect;


            float currentDist = map.GetDistance(actor.location, goal.TargetLocation);
            float dist = map.GetDistance(move.TargetLocation, goal.TargetLocation);

            return (currentDist - dist)*((map.LocationCount - dist)/ map.LocationCount);
        }

        return 0;
    }
}
