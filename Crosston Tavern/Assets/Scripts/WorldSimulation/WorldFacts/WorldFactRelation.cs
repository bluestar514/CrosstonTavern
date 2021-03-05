using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldFactRelation : WorldFact
{
    public StateRelation relation;
    public string owner;

    public WorldFactRelation(StateRelation relation, string owner)
    {
        this.relation = relation;
        this.owner = owner;
    }

    public override string ToString()
    {
        return "{"+owner + ":" + relation.ToString() + "}";
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new WorldFactRelation((StateRelation)relation.Bind(bindings, resources), bindings.BindString(owner));
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {

        ws.GetRelationshipsFor(relation.source).AddRelationTag(relation.target, relation.tag);

        return base.UpdateWorldState(ws);

    }

    
}
