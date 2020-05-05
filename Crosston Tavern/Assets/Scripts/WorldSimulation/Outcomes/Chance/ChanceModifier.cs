using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChanceModifier 
{
    public virtual float Chance(WorldState ws)
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

    public override float Chance(WorldState ws)
    {
        return chance;
    }
}

public class ChanceModifierRelation : ChanceModifier
{
    public EffectSocialChange socialState;
    public int boundry;
    public bool positive; 

    public ChanceModifierRelation(EffectSocialChange socialState, int boundry, bool positive)
    {
        this.socialState = socialState;
        this.boundry = boundry;
        this.positive = positive;
    }
    
    public override float Chance(WorldState ws)
    {
        Person source = ws.registry.GetPerson(socialState.SourceId);
        string target = socialState.TargetId;
        Relationship.RelationType type = socialState.RelationType;

        float currentRelValue = source.relationships.Get(target, type);

        //currentRelValue should be between 0 and 1, never negative
        //at the boundry value it should return .5
        //at deltaMax -> 1
        //at deltaMin -> 0

        currentRelValue -= socialState.DeltaMin;
        float max = socialState.DeltaMax - socialState.DeltaMin;

        currentRelValue = currentRelValue / max;

        if (!positive) return 1 - currentRelValue;
        else return currentRelValue;

    }
}