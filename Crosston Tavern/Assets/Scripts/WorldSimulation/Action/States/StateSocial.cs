using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSocial : State
{
    public string sourceId;
    public string targetId;
    public Relationship.RelationType axis;
    public int min;
    public int max;

    public StateSocial(string sourceId, string targetId, Relationship.RelationType axis, int min, int max)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateSocial(" +
            sourceId + "-(" + axis.ToString() + ")->" + targetId
            + ",{" + min + "~" + max + "})>";
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        float relValue = ws.map.GetPerson(source).relationships.Get(target, axis);

        return relValue <= max && relValue >= min;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        return new StateSocial(source, target, axis, min, max);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateSocial)) return new List<State>() { this, state };
        StateSocial stateSoc = (StateSocial)state;

        if (stateSoc.sourceId != sourceId ||
            stateSoc.targetId != targetId ||
            stateSoc.axis != axis) return new List<State>() { this, state };

        int min = Mathf.Max(this.min, stateSoc.min);
        int max = Mathf.Min(this.max, stateSoc.max);

        if (min > max) return new List<State>() { this, state };

        return new List<State>() {
            new StateSocial(sourceId, targetId, axis, min, max)
        };
    }
}