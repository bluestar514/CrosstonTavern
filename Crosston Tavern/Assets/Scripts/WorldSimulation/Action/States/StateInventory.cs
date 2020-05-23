using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateInventory : State
{
    public string ownerId;
    public string itemId;
}

public class StateInventoryStatic : StateInventory
{

    public int min;
    public int max;

    public StateInventoryStatic(string ownerId, string itemId, int min, int max)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateInventory(" + ownerId + ",{" + min + "~" + max + "}," + itemId + ")>";
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


        return new StateInventoryStatic(owner, string.Join(",", items), min, max);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateInventoryStatic)) return new List<State>() { this, state };
        StateInventoryStatic stateInv = (StateInventoryStatic) state;

        if(stateInv.itemId != itemId ||
            stateInv.ownerId != ownerId) return new List<State>() { this, state };

        int min = Mathf.Max(this.min, stateInv.min);
        int max = Mathf.Min(this.max, stateInv.max);

        if(min > max) return new List<State>() { this, state };

        return new List<State>() {
            new StateInventoryStatic(ownerId, itemId, min, max)
        };
    }
}


public class StateInventoryBound : StateInventory
{

    public string min;
    public string max;

    public StateInventoryBound(string ownerId, string itemId, string min, string max)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateInventory(" + ownerId + ",{" + min + "~" + max + "}," + itemId + ")>";
    }

    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        State state = this.Bind(bindings, resources);

        return state.InEffect(ws, bindings, resources);
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string boundItem = bindings.BindString(itemId);

        int i_min = int.Parse(bindings.BindString(min));
        int i_max = int.Parse(bindings.BindString(max));


        List<string> items = resources.BindString(boundItem);
        items.Sort();


        return new StateInventoryStatic(owner, string.Join(",", items), i_min, i_max);
    }
}
