using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VerbilizationEffect : VerbilizationInfo
{
    protected string verb;

    public VerbilizationEffect(string verb)
    {
        this.verb = verb;
    }

    virtual public string Verbilize(string actor, Effect effect)
    {
        return verb;
    }
}

public class VerbilizationEffectItemGather : VerbilizationEffect
{
    public VerbilizationEffectItemGather(string verb) : base(verb)
    {
    }

    //and caught 4 trout
    //found 2 strawberries
    public override string Verbilize(string actor, Effect effect)
    {
//        Debug.Log(effect + ":" + (effect is EffectInventory));


        if (effect is EffectInventory) {
            EffectInventory invEffect = (EffectInventory)effect;

            List<string> parts = new List<string>() {  verb, Mathf.Abs(invEffect.delta).ToString(), invEffect.itemId };

            return string.Join(" ", parts); //actor + " " + verb + " " + Mathf.Abs(invEffect.delta) + " " + invEffect.itemId;
        }

        return "";
    }

}

public class VerbilizationEffectSocialThreshold : VerbilizationEffect
{
    string negative;
    float threshold;

    public VerbilizationEffectSocialThreshold(string verb, string negative, float threshold) : base(verb)
    {
        this.negative = negative;
        this.threshold = threshold;
    }

    public override string Verbilize(string actor, Effect effect)
    {
        if (effect is EffectSocial) {
            EffectSocial socEffect = (EffectSocial)effect;

            if (socEffect.delta >= threshold) {
                return verb;
            } else return negative;
        }

        return "";
    }
}

