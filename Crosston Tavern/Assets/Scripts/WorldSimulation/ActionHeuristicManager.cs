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

    public ActionHeuristicManager(Person actor, Registry people, Map map)
    {
        this.actor = actor;
        this.people = people;
        this.map = map;
    }

    //Public Facing:

    public List<WeightedAction> GenerateWeightedOptions()
    {
        List<BoundAction> boundActions = GatherProvidedActionsFor();
        boundActions = FilterOnPrecondition(boundActions);
        List<WeightedAction> weightedActions = WeighOptions(boundActions);

        return weightedActions;
    }

    public ChosenAction ChooseAction(List<WeightedAction> choices)
    {
        choices = new List<WeightedAction>(choices.OrderByDescending(action => action.weight));

        int rand = Mathf.FloorToInt(UnityEngine.Random.value * 3);

        WeightedAction choice = choices[rand];
        choices.RemoveAt(rand);

        return new ChosenAction(choice, choices);
    }

    //Steps toward Generating Weighted Options:


    List<BoundAction> GatherProvidedActionsFor()
    {
        return map.GatherProvidedActionsForActorAt(actor.Id, actor.location);
    }

    List<BoundAction> FilterOnPrecondition(List<BoundAction> actions)
    {

        return new List<BoundAction>(from action in actions
                                     where action.SatisfiedPreconditions(
                                                         actor,
                                                         map.GetFeature(action.FeatureId),
                                                         map.GetLocation(action.LocationId))
                                     select action);

    }

    List<WeightedAction> WeighOptions(List<BoundAction> actions)
    {
        List<WeightedAction> weightedActions = new List<WeightedAction>();

        foreach (BoundAction action in actions) {
            Dictionary<string, List<string>> resources = GetActionResources(map, action, actor);

            List<Effect> boundEffects = action.GenerateKnownEffects(resources);

            weightedActions.Add(EvaluateAction(action, boundEffects));
        }

        return weightedActions;
    }


    WeightedAction EvaluateAction(BoundAction action, List<Effect> boundEffects)
    {
        List<WeightedAction.WeightRational> weightRationals = new List<WeightedAction.WeightRational>();
        foreach (Effect effect in boundEffects) {
            float effectLikelyhood = effect.EvaluateChance();

            foreach (MicroEffect subeffect in effect.effects) {
                foreach (KeyValuePair<MicroEffect, float> kvp in actor.goalPriorityDict) {
                    MicroEffect goal = kvp.Key;
                    float priority = kvp.Value;

                    float weight = priority * effectLikelyhood * EvaluateEffectTowardGoal(subeffect, goal);

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

    float EvaluateEffectTowardGoal(MicroEffect effect, MicroEffect goal)
    {
        goal = goal.BindEffect(actor.resources);

        if (goal is InvChange) {
            InvChange invGoal = (InvChange)goal;

            return EvaluateInvGoals(effect, invGoal);
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

    float EvaluateInvGoals(MicroEffect effect, InvChange goal)
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

            return 0;
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

            int dist = map.GetDistance(move.TargetLocation, goal.TargetLocation);

            return map.LocationCount - dist;
        }

        return 0;
    }
}
