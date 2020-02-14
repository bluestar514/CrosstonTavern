using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Person
{
    [SerializeField]
    private string id;
    public string Id { get => id; private set => id = value; }
    public string location;
    public Feature feature; //this is them as a feature, gets moved around the map.

    Dictionary<string, int> inventory;
    Dictionary<string, float> relationships;

    public Dictionary<MicroEffect, float> goalPriorityDict;

    Dictionary<string, List<string>> resources;

    public Person(string id)
    {
        Id = id;
    }

    public WeightedAction EvaluateAction(BoundAction action, List<Effect> boundEffects)
    {
        List<WeightedAction.WeightRational> weightRationals = new List<WeightedAction.WeightRational>();
        foreach(Effect effect in boundEffects) {
            float effectLikelyhood = effect.chanceModifier.Chance();

            foreach(MicroEffect subeffect in effect.effects) {
                foreach (KeyValuePair<MicroEffect, float> kvp in goalPriorityDict) {
                    MicroEffect goal = kvp.Key;
                    float priority = kvp.Value;

                    float weight = priority * effectLikelyhood * EvaluateEffectTowardGoal(subeffect, goal);

                    weightRationals.Add(new WeightedAction.WeightRational(subeffect, goal, weight));
                }
            }
        }

        float total = 0;
        foreach(WeightedAction.WeightRational wr in weightRationals) {
            total += wr.weight;
        }

        return new WeightedAction(action, total, weightRationals);

    }

    float EvaluateEffectTowardGoal(MicroEffect effect, MicroEffect goal)
    {
        goal = goal.BindEffect(resources);

        if (goal is InvChange) {
            InvChange invGoal = (InvChange)goal;

            return 1;
        }

        
        if(goal is Move) {
            Move locationGoal = (Move)goal;
            return 1;
        }

        
        if(goal is SocialChange) {
            SocialChange socialGoal = (SocialChange)goal;
            return 1;
        }

        return 0;
    }

    float EvaluateInvGoals(MicroEffect effect, InvChange goal)
    {
        if(effect is InvChange) {
            InvChange invChange = (InvChange)effect;

            int matches = 0;
            int count = 0;
            foreach(string item in goal.ItemId) {
                if (invChange.ItemId.Contains(item)) matches++;
                if (inventory.ContainsKey(item)) count += inventory[item];
            }

            if (matches == 0) return 0;

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

            
        }

        return 0;
    }

}
