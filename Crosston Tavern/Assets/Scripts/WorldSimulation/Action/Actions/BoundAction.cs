using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoundAction : WorldAction
{
    public string ActorId { get; private set; }
    public string FeatureId { get; private set; }
    public string LocationId { get; private set; }
    public BoundBindingCollection Bindings { get; private set; }

    public BoundAction(string id, int executionTime, Precondition preconditions, List<Outcome> potentialEffects, 
        string actorId, string featureId, string locationId, BoundBindingCollection Bindings, VerbilizationAction verbilizationInfo) :
        base(id, executionTime, preconditions, potentialEffects, verbilizationInfo)
    {
        this.ActorId = actorId;
        this.FeatureId = featureId;
        this.LocationId = locationId;
        if (Bindings == null) Bindings = new BoundBindingCollection(new List<BoundBindingPort>());
        this.Bindings = Bindings;

        name = ToString();
    }

    public BoundAction(GenericAction action, string actorId, string featureId, string locationId, BoundBindingCollection bindings, VerbilizationAction verbilizationInfo) : 
        this(action.Id, action.executionTime, action.preconditions, action.potentialOutcomes, actorId, featureId, locationId, bindings, verbilizationInfo)
    { }

    public BoundAction(BoundAction action, string actorId, string featureId, string locationId, BoundBindingCollection bindings, VerbilizationAction verbilizationInfo) :
        this(action.Id, action.executionTime, action.preconditions, action.potentialOutcomes, actorId, featureId, locationId, bindings, verbilizationInfo)
    { }

    public override string ToString()
    {
        string n = "<" + Id + "(" + ActorId + ", " + FeatureId + ")>";
        if (Bindings == null) return n;

        n = Bindings.BindString(n);
        
        return n;
    }

    public override bool Equals(object obj)
    {
        if (obj is BoundAction otherAction) {
            string thisActionId = Id;
            if (Bindings != null) thisActionId = Bindings.BindString(thisActionId);
            string otherActionId = otherAction.Id;
            if (otherAction.Bindings != null) otherActionId = otherAction.Bindings.BindString(otherActionId);

            return
                thisActionId == otherActionId &&
                ActorId == otherAction.ActorId &&
                FeatureId == otherAction.FeatureId;
        }

        return false;
    }

    public override int GetHashCode()
    {
        var hashCode = 1998656861;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ActorId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FeatureId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocationId);
        hashCode = hashCode * -1521134295 + EqualityComparer<BoundBindingCollection>.Default.GetHashCode(Bindings);
        return hashCode;
    }
}
