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
        string actorId, string featureId, string locationId, BoundBindingCollection Bindings) :
        base(id, executionTime, preconditions, potentialEffects)
    {
        this.ActorId = actorId;
        this.FeatureId = featureId;
        this.LocationId = locationId;
        if (Bindings == null) Bindings = new BoundBindingCollection(new List<BoundBindingPort>());
        this.Bindings = Bindings;

        name = ToString();
    }

    public BoundAction(GenericAction action, string actorId, string featureId, string locationId, BoundBindingCollection bindings): 
        this(action.Id, action.executionTime, action.preconditions, action.potentialOutcomes, actorId, featureId, locationId, bindings)
    { }

    public override string ToString()
    {
        string n = "<" + Id + "(" + ActorId + ", " + FeatureId + ")>";
        if (Bindings == null) return n;

        n = Bindings.BindString(n);
        
        return n;
    }


}
