using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierRelation : ChanceModifier
{
    public StateRelation state;
    public bool positive;

    public ChanceModifierRelation(StateRelation state, bool positive)
    {
        this.state = state;
        this.positive = positive;
    }

    public override float Chance(WorldState ws)
    {
        Person source = ws.map.GetPerson(state.source);
        string target = state.target;
        Relationship.RelationshipTag tag = state.tag;

        if (source.relationships.GetTag(target).Contains(tag) == positive) return 1;

        if (!Relationship.codifiedRelationRanges.ContainsKey(tag)) return 0;

        float chance = 1;
        Dictionary<Relationship.RelationType, float[]> ranges = Relationship.codifiedRelationRanges[tag];
        foreach(Relationship.RelationType axis in ranges.Keys) {
            if (Mathf.Abs(ranges[axis][0]) == Relationship.maxValue &&
                Mathf.Abs(ranges[axis][1]) == Relationship.maxValue) continue;

            int min = 0;
            int max = Relationship.maxValue;
            if (ranges[axis][0] < 0) min = -Relationship.maxValue;
            if (ranges[axis][1] < 0) max = 0;

            chance *= new ChanceModifierSocial(
                            new StateSocial(state.source, target, axis, min, (int)ranges[axis][0]),
                            positive)
                      .Chance(ws);
            chance *= new ChanceModifierSocial(
                            new StateSocial(state.source, target, axis, (int)ranges[axis][1], max),
                            !positive)
                      .Chance(ws);
        }

        return chance;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        string source = bindings.BindString(state.source);
        string target = bindings.BindString(state.target);


        return new ChanceModifierRelation(
                    new StateRelation(source, target, state.tag), 
                    positive);
    }

    public override List<Goal> MakeGoal(WorldState ws, float priority)
    {
        string source = state.source;
        string target = state.target;
        Relationship.RelationshipTag tag = state.tag;


        if (positive) {
            return new List<Goal>() {
                new GoalState(new StateRelation(source, target, tag), priority)
            };
        } else {
            return new List<Goal>() {
                new GoalState(new StateRelation(source, target, tag), -priority)
            };
        }

    }
}
