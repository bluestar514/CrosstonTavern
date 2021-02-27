using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldFactRelation : WorldFact
{
    public StateRelation relation;

    public WorldFactRelation(StateRelation relation)
    {
        this.relation = relation;
    }

    public override string ToString()
    {
        return "{" + relation.ToString() + "}";
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new WorldFactRelation((StateRelation)relation.Bind(bindings, resources));
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {

        ws.GetRelationshipsFor(relation.source).AddRelationTag(relation.target, relation.tag);

        return base.UpdateWorldState(ws);

    }

    
}
