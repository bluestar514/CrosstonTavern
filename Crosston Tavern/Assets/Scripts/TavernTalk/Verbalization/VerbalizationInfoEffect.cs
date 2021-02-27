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

    virtual public VerbilizationEffect Combine(VerbilizationEffect other, Effect effect)
    {
        return this;
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


        if (effect is EffectInventory) {
            EffectInventory invEffect = (EffectInventory)effect;
            string itemId = "";
            if (Mathf.Abs(invEffect.delta) == 1)
                itemId = VerbalizationDictionary.Replace(invEffect.itemId, VerbalizationDictionary.Form.singular);
            else 
                itemId = VerbalizationDictionary.Replace(invEffect.itemId, VerbalizationDictionary.Form.plural);

            List<string> parts = new List<string>() {  verb, Mathf.Abs(invEffect.delta).ToString(), itemId };

            return string.Join(" ", parts); //actor + " " + verb + " " + Mathf.Abs(invEffect.delta) + " " + invEffect.itemId;
        }

        return "";
    }
}

public class VerbilizationEffectSocialThreshold : VerbilizationEffect
{
    float threshold;
    bool greater;

    public VerbilizationEffectSocialThreshold(string verb, float threshold, bool greater) : base(verb)
    {
        this.threshold = threshold;
        this.greater = greater;
    }

    public override string Verbilize(string actor, Effect effect)
    {
        if (effect is EffectSocial socEffect) {
            if ((socEffect.delta >= threshold) == greater) {
                return verb;
            }
        }

        return "";
    }


    public override VerbilizationEffect Combine(VerbilizationEffect other, Effect effect)
    {
        if (effect is EffectSocial socEffect) {
            if((socEffect.delta >= threshold)==greater) {
                return this;
            } else return other;
        }

        return base.Combine(other, effect);
    }
}

