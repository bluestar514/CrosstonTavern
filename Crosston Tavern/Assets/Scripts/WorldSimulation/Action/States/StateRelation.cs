using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRelation : State
{
    public string source;
    public string target;
    public Relationship.RelationshipTag tag;

    public StateRelation(string source, string target, Relationship.RelationshipTag tag)
    {
        this.source = source;
        this.target = target;
        this.tag = tag;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(this.source);
        string target = bindings.BindString(this.target);

        return new StateRelation(source, target, tag);
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        StateRelation state = (StateRelation)Bind(bindings, resources);

        Person source = ws.registry.GetPerson(state.source);

        return source.relationships.RelationTagged(state.target, tag);
    }
}
