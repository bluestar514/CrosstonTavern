﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public List<MicroEffect> BindEffects(Dictionary<string, List<string>> resources)
    {
        return new List<MicroEffect>(from effect in effects
                                     select effect.BindEffect(resources));        
    }
}