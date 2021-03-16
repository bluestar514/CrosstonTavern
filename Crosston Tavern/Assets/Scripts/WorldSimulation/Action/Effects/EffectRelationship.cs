using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRelationship : Effect
{
    public string source;
    public string target;
    public Relationship.Tag tag;
    public bool bidirectional;
    public bool add;

    public EffectRelationship(string source, string target, Relationship.Tag tag, 
                                bool bidirectional, bool add, VerbilizationEffect verbilizationEffect = null)
    {
        this.source = source;
        this.target = target;
        this.tag = tag;
        this.bidirectional = bidirectional;
        this.add = add;
        verbalization = verbilizationEffect;
    }


    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (goal is GoalState goalState) {
            string sourceId = bindings.BindString(source);
            string targetId = bindings.BindString(target);
            float weight = 0;

            if (goalState.state is StateRelation state) {
                if (state.source != sourceId ||
                    state.target != targetId ||
                    state.tag != tag) return 0;

                Relationship relA = ws.GetRelationshipsFor(sourceId);
                Relationship relB = ws.GetRelationshipsFor(targetId);

                
                if (!relA.RelationTagged(target, tag) && add) weight += 1;
                if (relA.RelationTagged(target, tag) && !add) weight += 1;

                if (bidirectional) {
                    if (!relB.RelationTagged(source, tag) && add) weight += 1;
                    if (relB.RelationTagged(source, tag) && !add) weight += 1;

                    weight /= 2;
                }

                return weight;
            }
        }

        

        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie actor, BoundBindingCollection bindings, FeatureResources resources)
    {
        string sourceId = bindings.BindString(source);
        string targetId = bindings.BindString(target);


        if (add) {
            ws.GetRelationshipsFor(sourceId).AddRelationTag(targetId, tag);
            if (bidirectional)
                ws.GetRelationshipsFor(targetId).AddRelationTag(sourceId, tag);
        } else {
            ws.GetRelationshipsFor(sourceId).RemoveRelationTag(targetId, tag);
            if (bidirectional)
                ws.GetRelationshipsFor(targetId).RemoveRelationTag(sourceId, tag);
        }

        return new EffectRelationship(sourceId, targetId, tag, bidirectional, add, verbalization);
    }

    public override string ToString()
    {
        return "<EffectRelation(" +
            source + "," + target
            + "," + tag + ", bidirectional:" +bidirectional+", add:"+add+ ")>";
    }
}
