﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoundAction : GenericAction
{
    public string ActorId { get; private set; }
    public string FeatureId { get; private set; }
    public string LocationId { get; private set; }

    public BoundAction(string id, List<Condition> preconditions, List<Effect> potentialEffects, 
        string actorId, string featureId, string locationId):base(id, preconditions, potentialEffects)
    {
        this.ActorId = actorId;
        this.FeatureId = featureId;
        this.LocationId = locationId;

        name = ToString();
    }

    public BoundAction(GenericAction action, string actorId, string featureId, string locationId): 
        this(action.Id, action.preconditions, action.potentialEffects, actorId, featureId, locationId)
    { }

    public override string ToString()
    {
        return "<"+Id+"("+ActorId+", "+ FeatureId+ ")>";
    }


    public List<Effect> GenerateKnownEffects(Dictionary<string, List<string>> resources)
    {
        return new List<Effect>(
            from effect in potentialEffects
            select new Effect(effect.chanceModifier, effect.BindEffects(resources))
        );
    }
}