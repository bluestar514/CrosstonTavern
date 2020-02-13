using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChanceModifier 
{
    public virtual float Chance()
    {
        return 1;
    }
}

public class ChanceModifierSimple: ChanceModifier
{
    float chance;

    public ChanceModifierSimple(float chance)
    {
        this.chance = chance;
    }

    public override float Chance()
    {
        return chance;
    }
}