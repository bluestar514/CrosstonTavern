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

    public override string Verbalize(string speakerId, string listenerId, bool goal)
    {
        string subject = ownerId;
        if (subject == speakerId) subject = "I";
        if (subject == listenerId) subject = "you";
        subject = VerbalizationDictionary.Replace(subject);

        string count = "some";
        if(min != max) {
            if (min == 1 && max > 100) count = "some";
            if (min > 20) count = "a lot of";
            if (min > 5) count = "a few";
        } else {
            if (min > 10) count = "a ton of";
            if (min > 5) count = "a lot of";
            if (min > 2) count = "a few";
            if (min == 1) count = "a";
        }

        string item = itemId;
        if(count == "a") item = VerbalizationDictionary.Replace(item, VerbalizationDictionary.Form.singular);
        else item = VerbalizationDictionary.Replace(item, VerbalizationDictionary.Form.plural);

        List<string> parts;
        if (goal) {
            if (subject == "I" || subject == "you")
                parts = new List<string>() { "to have", count, item };
            else
                parts = new List<string>() { subject, "to have", count, item };
        } else {
            if (subject == "I")
                parts = new List<string>() { "I have", count, item };
            else
                parts = new List<string>() { subject, " has ", count, item };
        }

        return string.Join(" ", parts);
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
