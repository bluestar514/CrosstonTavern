using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSocial : Effect
{
    public string sourceId;
    public string targetId;
    public Relationship.Axis axis;

    public virtual int delta { get; protected set; }
    public override string ToString()
    {
        return "<EffectSocial(" + sourceId + "-(" + axis.ToString() + ")->" + targetId;
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (goal is GoalState goalState) {
            if (goalState.state is StateSocial state) {



                string source = bindings.BindString(sourceId);
                string target = bindings.BindString(targetId);
                Relationship.Axis axis = this.axis;

                string goalSource = state.sourceId;
                string goalTarget = state.targetId;
                Relationship.Axis goalAxis = state.axis;

                if (source != goalSource ||
                    target != goalTarget ||
                    axis != goalAxis) return 0;

                Relationship rel = ws.GetRelationshipsFor(source);
                int value = rel.Get(target, axis);

                return FinishWeighing(value, state.min, state.max);
            }
        }
        return 0;
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

        return new EffectSocialStatic(sourceId, targetId, axis, delta, verbalization);
    }

    public override Effect Combine(Effect other)
    {
        if (other is EffectSocial) {
            EffectSocial otherSoc = (EffectSocial)other;

            if (sourceId == otherSoc.sourceId &&
                targetId == otherSoc.targetId &&
                axis == otherSoc.axis) {
                Effect effect = new EffectSocialStatic(sourceId, targetId, axis, delta + otherSoc.delta);

                if (verbalization == null) effect.verbalization = other.verbalization;
                else effect.verbalization = verbalization.Combine(other.verbalization, effect);

                return effect;
            }
        }

        return null;
    }
}

public class EffectSocialVariable : EffectSocial
{
    public int min;
    public int max;

    public override int delta {
        get => Mathf.RoundToInt((UnityEngine.Random.value * (max - min)) + min);
    }

    public EffectSocialVariable(string sourceId, string targetId, Relationship.Axis axis, int min, int max, VerbilizationEffect verbilizationEffect = null)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.min = min;
        this.max = max;

        verbalization = verbilizationEffect;

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

    public EffectSocialStatic(string sourceId, string targetId, Relationship.Axis axis, int delta, VerbilizationEffect verbilizationEffect = null)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.delta = delta;

        verbalization = verbilizationEffect;

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