﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BoundAction : GenericAction
{
    public string ActorId { get; private set; }
    public string FeatureId { get; private set; }
    public string LocationId { get; private set; }

    public BoundAction(string id, int executionTime, List<Condition> preconditions, List<Effect> potentialEffects, 
        string actorId, string featureId, string locationId):base(id, executionTime, preconditions, potentialEffects)
    {
        this.ActorId = actorId;
        this.FeatureId = featureId;
        this.LocationId = locationId;

        name = ToString();
    }

    public BoundAction(GenericAction action, string actorId, string featureId, string locationId): 
        this(action.Id, action.executionTime, action.preconditions, action.potentialEffects, actorId, featureId, locationId)
    { }

    public override string ToString()
    {
        return "<"+Id+"("+ActorId+", "+ FeatureId+ ")>";
    }

    public bool SatisfiedPreconditions(WorldState ws)
    {
        Person actor = ws.registry.GetPerson(ActorId);
        Feature feature = ws.map.GetFeature(FeatureId);
        Location location = ws.map.GetLocation(LocationId);

        return base.SatisfiedPreconditions(actor, feature, location, ws);
    }

    public List<Condition> GetBoundConditions(WorldState ws)
    {
        return base.GetBoundConditions(ws, ActorId, FeatureId, LocationId);
    }

        public List<Effect> GenerateExpectedEffects(WorldState ws)
    {
        return base.GenerateExpectedEffects(ws, ActorId, FeatureId, LocationId);
    }
}
