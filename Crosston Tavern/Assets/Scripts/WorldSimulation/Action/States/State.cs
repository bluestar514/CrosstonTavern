using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    public string id;
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
}
