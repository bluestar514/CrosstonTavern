using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedAction : BoundAction
{
    public float weight;
    public List<WeightRational> weightRationals;

    public List<Outcome> expectedEffects;

    public WeightedAction(GenericAction action, 
        string actorId, string featureId, string locationId, List<BoundBindingPort> bindings,
        float weight, List<WeightRational> weightRationals, List<Outcome> expectedEffects) : 
        base(action, actorId, featureId, locationId, bindings)
    {
        this.weight = weight;
        this.weightRationals = weightRationals;
        this.expectedEffects = expectedEffects;

        name = ToString();
    }

    public WeightedAction(string id, int executionTime, List<Condition> preconditions, List<Outcome> potentialEffects, 
        string actorId, string featureId, string locationId, List<BoundBindingPort> bindings,
        float weight, List<WeightRational> weightRationals, List<Outcome> expectedEffects) : 
        base(id, executionTime, preconditions, potentialEffects, actorId, featureId, locationId, bindings)
    {
        this.weight = weight;
        this.weightRationals = weightRationals;
        this.expectedEffects = expectedEffects;

        name = ToString();
    }
        
    public WeightedAction(BoundAction boundAction, float weight, List<WeightRational> weightRationals, List<Outcome> expectedEffects) :
        this(boundAction.Id, boundAction.executionTime, boundAction.preconditions, boundAction.potentialEffects, 
            boundAction.ActorId, boundAction.FeatureId, boundAction.LocationId, null, weight, weightRationals, expectedEffects)
    {}


    public override string ToString()
    {
        return "<" + Id + "(" + ActorId + ", " + FeatureId + "):"+ weight +">";
    }

    [System.Serializable]
    public class WeightRational
    {
        [SerializeField]
        string name;

        public Effect effect;
        public Effect goal;
        public float weight;

        public WeightRational(Effect effect, Effect goal, float weight)
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
