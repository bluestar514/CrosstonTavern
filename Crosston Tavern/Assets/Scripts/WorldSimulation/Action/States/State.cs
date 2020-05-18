using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State 
{
    public string id;


    public virtual bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        return false;
    }


    public virtual State Bind(BoundBindingCollection bindings, FeatureResources resources) { return this; }
}

public class StateSocial: State
{
    public string sourceId;
    public string targetId;
    public Relationship.RelationType axis;
    public int min;
    public int max;

    public StateSocial(string sourceId, string targetId, Relationship.RelationType axis, int min, int max)
    {
        this.sourceId = sourceId;
        this.targetId = targetId;
        this.axis = axis;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateSocial(" + 
            sourceId + "-(" + axis.ToString() + ")->" + targetId
            + ",{" + min + "~" + max + "})>";
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        float relValue = ws.registry.GetPerson(source).relationships.Get(target, axis);

        return relValue <= max && relValue >= min;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string source = bindings.BindString(sourceId);
        string target = bindings.BindString(targetId);

        return new StateSocial(source, target, axis, min, max);
    }
}

public class StateInventory: State
{
    public string ownerId;
    public string itemId;
    public int min;
    public int max;

    public StateInventory(string ownerId, string itemId, int min, int max)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateInventory(" + ownerId+ ",{" + min + "~" + max + "}," + itemId + ")>";
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string boundItem = bindings.BindString(itemId);

        List<string> items = resources.BindString(boundItem);

        Inventory inv = ws.map.GetFeature(owner).inventory;


        int count = (from item in items
                     select inv.GetInventoryCount(item)).Sum();

        return count <= max && count >= min;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string boundItem = bindings.BindString(itemId);

        List<string> items = resources.BindString(boundItem);
        items.Sort();


        return new StateInventory(owner, string.Join(",", items), min, max);
    }
}

public class StatePosition : State
{
    public string moverId;
    public string locationId;

    public StatePosition(string moverId, string locationId)
    {
        this.moverId = moverId;
        this.locationId = locationId;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StatePosition(" + moverId + "," + locationId + ")>";
    }


    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {

        string mover = bindings.BindString(moverId);
        string location = bindings.BindString(locationId);

        List<string> potentialIds = resources.BindString(location);

        Feature f = ws.map.GetFeature(moverId);

        return potentialIds.Any(l => l == location);
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string mover = bindings.BindString(moverId);
        string location = bindings.BindString(locationId);

        List<string> potentialIds = resources.BindString(location);

        return new StatePosition(mover, string.Join(",", potentialIds));
    }
}
