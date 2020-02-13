using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect 
{
    public ChanceModifier chanceModifier;
    public List<MicroEffect> effects;

    public Effect(ChanceModifier chanceModifier, List<MicroEffect> effects)
    {
        this.chanceModifier = chanceModifier;
        this.effects = effects;
    }
}
