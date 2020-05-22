using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSocial : Effect
{
    public string sourceId;
    public string targetId;
    public Relationship.RelationType axis;

    public virtual int delta { get; protected set; }
    public override string ToString()
    {
        return "<EffectSocial(" + sourceId + "-(" + axis.ToString() + ")->" + targetId;
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (!(goal.state is StateSocial)) return 0;
        StateSocial state = (StateSocial)goal.state;


        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);
        Relationship.RelationType axis = this.axis;

        string goalSource = state.sourceId;
        string goalTarget = state.targetId;
        Relationship.RelationType goalAxis = state.axis;

        if (source != goalSource ||
            target != goalTarget ||
            axis != goalAxis) return 0;

        Relationship rel = ws.GetRelationshipsFor(source);
        int value = rel.Get(target, axis);

        return FinishWeighing(value, state.min, state.max);

    }

    protected virtual float FinishWeighing(int value, int min, int max)
    {
        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie actor, BoundBindingCollection bindings, FeatureResources resources)
    {

        string sourceId = bindings.BindString(this.sourceId);
        string targetId = bindings.BindString(this.targetId);

        Relationship rel = ws.GetRelationshipsFor(sourceId);
        rel.Increase(targetId, axis, delta);

        return new EffectSocialStatic(sourceId, targetId, axis, delta);
    }
}

public class EffectSocialVariable : EffectSocial
{
    public int min;
    public int max;

    public override int delta {
        get => Mathf.RoundToInt((UnityEngine.Random.value * (max - min)) + min);
    }

    public EffectSocialVariable(string sourceId, string targetId, Relationship.RelationType axis, int min, int max)
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
        return base.ToString() + ",{" + min + "~" + max + "})>";
    }

    protected override float FinishWeighing(int value, int goalStateMin, int goalStateMax)
    {
        return (CountInRange(value, this.min, goalStateMin, goalStateMax) +
                CountInRange(value, this.max, goalStateMin, goalStateMax))
                / 2;
    }
}

public class EffectSocialStatic : EffectSocial
{

    public EffectSocialStatic(string sourceId, string targetId, Relationship.RelationType axis, int delta)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.delta = delta;
        id = ToString();
    }

    public override string ToString()
    {
        return base.ToString() + "," + delta + ")>";
    }

    protected override float FinishWeighing(int value, int min, int max)
    {
        return CountInRange(value, delta, min, max);
    }
}