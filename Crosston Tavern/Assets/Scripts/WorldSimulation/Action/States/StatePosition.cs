﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class StatePosition : State
{
    public string moverId;
    public string locationId;

    public StatePosition(string moverId, string locationId)
    {
        this.moverId = moverId;
        this.locationId = locationId;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StatePosition(" + moverId + "," + locationId + ")>";
    }


    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {

        string mover = bindings.BindString(moverId);
        string location = bindings.BindString(locationId);

        List<string> potentialIds = resources.BindString(location);

        Feature f = ws.map.GetFeature(moverId);

        return potentialIds.Any(l => l == location);
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string mover = bindings.BindString(moverId);
        string location = bindings.BindString(locationId);

        List<string> potentialIds = resources.BindString(location);

        return new StatePosition(mover, string.Join(",", potentialIds));
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StatePosition)) return new List<State>() { this, state };
        StatePosition stateSoc = (StatePosition)state;

        if (stateSoc.moverId != moverId ||
            stateSoc.locationId != locationId) return new List<State>() { this, state };

        return new List<State>() {
            new StatePosition(moverId, locationId)
        };
    }
}