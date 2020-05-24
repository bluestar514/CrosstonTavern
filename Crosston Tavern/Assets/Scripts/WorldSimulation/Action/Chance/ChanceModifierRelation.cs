using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierRelation : ChanceModifier
{
    public StateSocial socialState;
    public bool positive;

    public ChanceModifierRelation(StateSocial socialState, bool positive)
    {
        this.socialState = socialState;
        this.positive = positive;
    }

    public override float Chance(WorldState ws)
    {
        Person source = ws.registry.GetPerson(socialState.sourceId);
        string target = socialState.targetId;
        Relationship.RelationType type = socialState.axis;

        float currentRelValue = source.relationships.Get(target, type);

        //currentRelValue should be between 0 and 1, never negative
        //at deltaMax -> 1
        //at deltaMin -> 0

        currentRelValue = Mathf.Max(0, currentRelValue - socialState.min);
        float max = socialState.max - socialState.min;

        currentRelValue = currentRelValue / max;

        if (!positive) return 1 - currentRelValue;
        else return currentRelValue;

    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        string source = bindings.BindString(socialState.sourceId);
        string target = bindings.BindString(socialState.targetId);


        return new ChanceModifierRelation(new StateSocial(source, target, socialState.axis, socialState.min, socialState.max), positive);
    }

    public override List<Goal> MakeGoal(WorldState ws, float priority)
    {
        string source = socialState.sourceId;
        string target = socialState.targetId;
        Relationship.RelationType axis = socialState.axis;
        int min = socialState.min;
        int max = socialState.max;

        if (positive)
            return new List<Goal>() { new Goal(new StateSocial(source, target, axis, max, 1000000), priority) };
        else
            return new List<Goal>() { new Goal(new StateSocial(source, target, axis, -1000000, min), priority) };
    }
}