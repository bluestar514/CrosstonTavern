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
    Person actor;
    Registry people;
    Map map;
    WorldState WS;
    ActionBuilder ab;
    public ActionHeuristicManager(Person actor, WorldState ws)
    {
        this.actor = actor;
        WS = ws;
        people = WS.registry;
        map = WS.map;

        ab = new ActionBuilder(WS, actor);
    }

    //Public Facing:

    /// <summary>
    /// Create Weights for all potential Bound Actions (ie, make the Weighted Actions)
    /// </summary>
    /// <param name="location"></param>
    /// <param name="depth"></param>
    /// <param name="considerPreconditions"></param>
    /// <returns></returns>
    public List<WeightedAction> GenerateWeightedOptions(string location = "", int depth = 0, bool considerPreconditions = true)
    {
        if (depth >= 5) return new List<WeightedAction>();


        List<BoundAction> boundActions = ab.BindActions(location);
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

        

        return new ChosenAction(choice,GetRejectedOnPrecondition(ab.BindActions()), choices);
    }

    /// <summary>
    /// Pick the "Best" next action, from all possible weighted actions
    /// </summary>
    /// <returns></returns>
    public ChosenAction ChooseAction()
    {
        List<WeightedAction> choices = GenerateWeightedOptions();

        return ChooseAction(choices);
    }

    //Steps toward Generating Weighted Options:

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

            List<Outcome> boundEffects = action.GenerateExpectedEffects(WS);

            weightedActions.Add(EvaluateAction(action, boundEffects, depth));
        }

        return weightedActions;
    }


    WeightedAction EvaluateAction(BoundAction action, List<Outcome> boundEffects, int depth)
    {
        List<WeightedAction.WeightRational> weightRationals = new List<WeightedAction.WeightRational>();
        foreach (Outcome effect in boundEffects) {
            float effectLikelyhood = effect.EvaluateChance(WS);

            foreach (Effect subeffect in effect.effects) {
                foreach (Goal g in actor.goals) {
                    Effect goal = g.state;
                    float priority = g.priority;

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

    public float EvaluateEffectTowardGoal(Effect effect, Effect goal, int depth)
    {

        goal = goal.BindEffect(actor.resources);

        if (goal is EffectInvChange) {
            EffectInvChange invGoal = (EffectInvChange)goal;

            return EvaluateInvGoals(effect, invGoal, depth);
        }


        if (goal is EffectMove) {
            EffectMove locationGoal = (EffectMove)goal;
            return EvaluateMoveGoals(effect, locationGoal);
        }


        if (goal is EffectSocialChange) {
            EffectSocialChange socialGoal = (EffectSocialChange)goal;
            return EvaluateSocialGoals(effect, socialGoal);
        }

        return 0;
    }

    float EvaluateInvGoals(Effect effect, EffectInvChange goal, int depth)
    {

        if (effect is EffectInvChange) {
            EffectInvChange invChange = (EffectInvChange)effect;

            //stop if the inventory this would add to is not the one we want
            if (invChange.InvOwner != goal.InvOwner) return 0;
            Inventory inventory = WS.GetInventory(goal.InvOwner);


            //stop if the item being changed isn't what we are looking for
            int matches = 0;
            int count = 0;
            foreach (string item in goal.ItemId) {
                if (invChange.ItemId.Contains(item)) matches++;
                count += inventory.GetInventoryCount(item);
            }
            if (matches == 0) return 0;

            //Determine how much this helps us
            float DeltaAverage = (invChange.DeltaMin + invChange.DeltaMax) / 2;
            float weight = 0;
            if ((count < goal.DeltaMin && invChange.DeltaMin >= 0 && invChange.DeltaMax > 0)
                || (count > goal.DeltaMax && invChange.DeltaMax <= 0 && invChange.DeltaMin < 0))// moving in the right direction
                weight += Mathf.Abs(DeltaAverage);
            else weight -= Mathf.Abs(DeltaAverage); //or we are moving in the wrong direction, and account for that too!
            if (count + DeltaAverage <= goal.DeltaMax && count + DeltaAverage >= goal.DeltaMin)
                weight += Mathf.Abs(DeltaAverage);

            return weight;

        }

        return 0;
    }

    float EvaluateMoveGoals(Effect effect, EffectMove goal)
    {
        if (effect is EffectMove) {
            EffectMove move = (EffectMove)effect;


            float currentDist = map.GetDistance(actor.location, goal.TargetLocation);
            float dist = map.GetDistance(move.TargetLocation, goal.TargetLocation);

            return (currentDist - dist)*((map.LocationCount - dist)/ map.LocationCount);
        }

        return 0;
    }

    float EvaluateSocialGoals(Effect effect, EffectSocialChange goal)
    {
        if (effect is EffectSocialChange) {
            EffectSocialChange socChange = (EffectSocialChange)effect;

            //stop if the source or target don't match or the value changing isn't what we are looking for
            if (!checkIfPeopleMatch(socChange.SourceId, goal.SourceId)) return 0;
            if (!checkIfPeopleMatch(socChange.TargetId, goal.TargetId)) return 0;
            if (socChange.RelationType != goal.RelationType) return 0;

            Relationship relations = WS.GetRelationshipsFor(socChange.SourceId);

            float value = relations.Get(socChange.TargetId, socChange.RelationType);

            //Determine how much this helps us
            float DeltaAverage = (socChange.DeltaMin + socChange.DeltaMax) / 2;
            float weight = 0;
            if ((value < goal.DeltaMin && socChange.DeltaMin >= 0 && socChange.DeltaMax > 0)
                || (value > goal.DeltaMax && socChange.DeltaMax <= 0 && socChange.DeltaMin < 0))// moving in the right direction
                weight += Mathf.Abs(DeltaAverage);
            if (value + DeltaAverage <= goal.DeltaMax && value + DeltaAverage >= goal.DeltaMin)
                weight += Mathf.Abs(DeltaAverage);

            return weight;

        }

        return 0;
    }


    bool checkIfPeopleMatch(string effect, string goal)
    {
        effect = effect.Replace("person_", "");
        goal = goal.Replace("person_", "");

        return effect == goal;
    }
}
