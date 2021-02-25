using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAvailable : State
{
    string actor;
    WorldTime start;
    WorldTime end;

    public StateAvailable(string actor, WorldTime start, WorldTime end )
    {
        this.start = start;
        this.end = end;
        this.actor = actor;
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string actor = bindings.BindString(this.actor);

        return ws.map.GetPerson(actor).schedule.TimeFree(start, end);
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string actor = bindings.BindString(this.actor);

        return new StateAvailable(actor, start, end);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateAvailable)) return base.Combine(state);
        StateAvailable stateAv = (StateAvailable)state;

        if(this.actor != stateAv.actor ||
            this.start != stateAv.start ||
            this.end != stateAv.end) return base.Combine(state);

        return new List<State>() { this };
    }

    public override string ToString()
    {
        return "<StateAvailable(" + actor + ", " + start + "," + end + ")>";
    }
}

public class StateAvailableNow: State
{
    string actor;
    WorldTime length;

    public StateAvailableNow(string actor, WorldTime length)
    {
        this.actor = actor;
        this.length = length;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string actor = bindings.BindString(this.actor);

        return new StateAvailableNow(actor, length);
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        StateAvailable state = new StateAvailable(actor, new WorldTime(ws.Time), ws.Time + length);

        return state.InEffect(ws, bindings, resources);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateAvailableNow)) return base.Combine(state);
        StateAvailableNow stateAv = (StateAvailableNow)state;

        if (this.actor != stateAv.actor ||
            this.length != stateAv.length) return base.Combine(state);

        return new List<State>() { this };
    }

    public override string ToString()
    {
        return "<StateAvailableNow(" + actor + ", " + length + ")>";
    }
}
