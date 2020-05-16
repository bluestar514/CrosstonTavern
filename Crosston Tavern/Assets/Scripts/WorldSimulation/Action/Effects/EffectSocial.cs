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
}