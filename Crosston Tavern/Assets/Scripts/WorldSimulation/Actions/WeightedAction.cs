using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedAction : BoundAction
{
    public WeightedAction(GenericAction action, string actorId, string featureId, string locationId) : 
        base(action, actorId, featureId, locationId)
    {
    }

    public WeightedAction(string id, List<Condition> preconditions, List<Effect> potentialEffects, 
        string actorId, string featureId, string locationId) : 
        base(id, preconditions, potentialEffects, actorId, featureId, locationId)
    {
    }
}
