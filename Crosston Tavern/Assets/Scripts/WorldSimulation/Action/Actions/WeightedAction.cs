﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedAction : BoundAction
{
    public float weight;
    public List<WeightRational> weightRationals;

    public List<Outcome> expectedEffects;

    public WeightedAction(string id, int executionTime, Precondition preconditions, List<Outcome> potentialEffects, 
        string actorId, string featureId, string locationId, BoundBindingCollection bindings,
        float weight, List<WeightRational> weightRationals) : 
        base(id, executionTime, preconditions, potentialEffects, actorId, featureId, locationId, bindings)
    {
        this.weight = weight;
        this.weightRationals = weightRationals;

        name = ToString();
    }
        
    public WeightedAction(BoundAction boundAction, float weight, List<WeightRational> weightRationals) :
        this(boundAction.Id, boundAction.executionTime, boundAction.preconditions, boundAction.potentialEffects, 
            boundAction.ActorId, boundAction.FeatureId, boundAction.LocationId, boundAction.Bindings, weight, weightRationals)
    {}


    public override string ToString()
    {
        string n = "<" + Id + "(" + ActorId + ", " + FeatureId + ")>";
        if (Bindings == null) return n;

        return Bindings.BindString(n);
    }

    public string VerboseString()
    {
        string str = ToString() +
            "\nOutcomes:\n\t" + string.Join("\n\t", potentialEffects) +
            "\nBindings:\n\t" + string.Join("\n\t", Bindings.bindings) +
            "\nRational:\n\t" + string.Join("\n\t", weightRationals);

        return Bindings.BindString(str);
    }

    [System.Serializable]
    public class WeightRational
    {
        [SerializeField]
        string name;

        public Effect effect;
        public State goal;
        public float weight;

        public WeightRational(Effect effect, State goal, float weight)
        {
            this.effect = effect;
            this.goal = goal;
            this.weight = weight;

            name = ToString();
        }

        public override string ToString()
        {
            return "{"+goal+", "+effect+": "+weight+"}";
        }
    }
}