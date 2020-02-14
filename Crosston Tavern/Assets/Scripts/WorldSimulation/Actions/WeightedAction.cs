using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedAction : BoundAction
{
    public float weight;
    public List<WeightRational> weightRationals;

    public WeightedAction(GenericAction action, 
        string actorId, string featureId, string locationId, 
        float weight, List<WeightRational> weightRationals) : 
        base(action, actorId, featureId, locationId)
    {
        this.weight = weight;
        this.weightRationals = weightRationals;

        name = ToString();
    }

    public WeightedAction(string id, List<Condition> preconditions, List<Effect> potentialEffects, 
        string actorId, string featureId, string locationId, 
        float weight, List<WeightRational> weightRationals) : 
        base(id, preconditions, potentialEffects, actorId, featureId, locationId)
    {
        this.weight = weight;
        this.weightRationals = weightRationals;
        name = ToString();
    }
        
    public WeightedAction(BoundAction boundAction, float weight, List<WeightRational> weightRationals) :
        this(boundAction.Id, boundAction.preconditions, boundAction.potentialEffects, 
            boundAction.ActorId, boundAction.FeatureId, boundAction.LocationId, weight, weightRationals)
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

        public MicroEffect effect;
        public MicroEffect goal;
        public float weight;

        public WeightRational(MicroEffect effect, MicroEffect goal, float weight)
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
