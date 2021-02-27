using System.Collections.Generic;

[System.Serializable]
public class WorldFactResource: WorldFact { 

    public string featureId;
    public string resourceId;
    public string potentialBinding;

    public WorldFactResource(string featureId, string resourceId, string potentialBinding)
    {
        this.featureId = featureId;
        this.resourceId = resourceId;
        this.potentialBinding = potentialBinding;

        id = ToString();
    }

    public override string ToString()
    {
        return "{" + featureId + ":(" + resourceId + "," + potentialBinding + ")}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactResource)) return false;

        WorldFactResource fact = (WorldFactResource)obj;
        if (this.featureId == fact.featureId &&
            this.resourceId == fact.resourceId &&
            this.potentialBinding == fact.potentialBinding) return true;

        return false;
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string featureId = bindings.BindString(this.featureId);
        string potentialBinding = bindings.BindString(this.potentialBinding);
        
        return new WorldFactResource(featureId, this.resourceId, potentialBinding);
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        Feature feature = ws.map.GetFeature(featureId);

        feature.relevantResources.Add(resourceId, potentialBinding);

        return base.UpdateWorldState(ws);
    }

    public override int GetHashCode()
    {
        var hashCode = 1424682090;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(featureId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(resourceId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(potentialBinding);
        return hashCode;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws=null)
    {
        return "There is " + potentialBinding + " at " + featureId;
    }
}
