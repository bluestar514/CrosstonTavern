using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WorldFact
{
    [SerializeField]
    protected string id = "{WorldFact:Generic}";

    public virtual WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        return this;
    }

    public virtual List<WorldFact> UpdateWorldState(WorldState ws) {
        return new List<WorldFact>() { this };
    }


    public virtual string Verbalize(string speaker, string listener, WorldState ws=null)
    {
        return id;
    }
}
