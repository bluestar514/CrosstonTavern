using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WorldFact
{
    public string featureId;
    public string resourceId;
    public string potentialBinding;

    public WorldFact(string featureId, string resourceId, string potentialBinding)
    {
        this.featureId = featureId;
        this.resourceId = resourceId;
        this.potentialBinding = potentialBinding;
    }

    public override string ToString()
    {
        return "{" + featureId + ":(" + resourceId + "," + potentialBinding + ")}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFact)) return false;

        WorldFact fact = (WorldFact)obj;
        if (this.featureId == fact.featureId &&
            this.resourceId == fact.resourceId &&
            this.potentialBinding == fact.potentialBinding) return true;

        return false;
    }
}
