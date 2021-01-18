using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectInventory : Effect
{
    public string ownerId;
    public virtual string itemId { get; protected set; }
    public virtual int delta { get; protected set; }
    public override string ToString()
    {
        return "<EffectInventory("+ownerId;
    }


    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if (!(goal.state is StateInventory)) return 0;
        StateInventoryStatic state = (StateInventoryStatic)((StateInventory)goal.state).Bind(bindings, resources);

        string goalInvOwner = state.ownerId;
        string goalItem = state.itemId;

        string owner = bindings.BindString(ownerId);

        if (goalInvOwner != owner.Replace("person_", "")) return 0;
        Inventory inv = ws.GetInventory(owner);


        return FinishWeighing(goalItem, inv, bindings, resources, state.min, state.max);
    }

    protected virtual float FinishWeighing(string goalItem, Inventory inv, BoundBindingCollection bindings, FeatureResources resources, int min, int max)
    {
        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie actor, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string itemid = bindings.BindString(itemId);

        List<string> items = resources.BindString(itemid);
        itemid = items[Mathf.FloorToInt(UnityEngine.Random.value * items.Count)];

        int num = delta;

        Inventory inv = ws.GetInventory(owner);
        inv.ChangeInventoryContents(num, itemid);

        Effect realizedEffect = new EffectInventoryStatic(owner, itemid, num);

        return realizedEffect;
    }

    public override Effect Combine(Effect other)
    {
        if(other is EffectInventory) {
            EffectInventory otherInv = (EffectInventory)other;

            if(ownerId == otherInv.ownerId &&
                itemId == otherInv.itemId) {
                return new EffectInventoryStatic(ownerId, itemId, delta + otherInv.delta);
            }
        }

        return null;
    }
}

public class EffectInventoryVariable : EffectInventory
{
    public List<string> itemIds= new List<string>();
    public int min;
    public int max;

    public override int delta {
        get => Mathf.RoundToInt((UnityEngine.Random.value * (max - min)) + min); }

    public override string itemId {
        get => itemIds[Mathf.FloorToInt(UnityEngine.Random.value * itemIds.Count)];
    }

    public EffectInventoryVariable(string ownerId, List<string> itemIds, int min, int max)
    {
        this.ownerId = ownerId;
        this.itemIds = new List<string>(itemIds);
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return base.ToString()+ ",{"+min+"~"+max+"}," + "{"+ string.Join(",", itemIds)+"})>";
    }

    protected override float FinishWeighing(string goalItem, Inventory inv, BoundBindingCollection bindings, FeatureResources resources, int min, int max)
    {
        List<string> items = new List<string>();
        foreach (string item in from item in itemIds
                                select bindings.BindString(item)) {
            items.AddRange(resources.BindString(item));
        }

        if (!items.Contains(goalItem)) return 0;
        int count = inv.GetInventoryCount(goalItem);

        return (CountInRange(count, this.min, min, max) +
                CountInRange(count, this.max, min, max))
                / 2;
    }
}

public class EffectInventoryStatic : EffectInventory
{

    public EffectInventoryStatic(string ownerId, string itemId, int delta)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.delta = delta;

        id = ToString();
    }

    public override string ToString()
    {
        return base.ToString() + "," + delta + "," + itemId + ")>";
    }

    protected override float FinishWeighing(string goalItem, Inventory inv, BoundBindingCollection bindings, FeatureResources resources, int min, int max)
    {
        string item = bindings.BindString(itemId);
        if (item != goalItem) return 0;

        
        int count = inv.GetInventoryCount(item);

        return CountInRange(count, delta, min, max);
    }
}


public class EffectInventoryBound: EffectInventory
{
    string s_delta;
    public override int delta {
        get => 0;
    }


    public EffectInventoryBound(string ownerId, string itemId, string delta)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.s_delta = delta;
    }

    public override string ToString()
    {
        return base.ToString() + "," + delta + "," + itemId + ")>";
    }

    public EffectInventoryStatic Bind(BoundBindingCollection bindings)
    {
        return new EffectInventoryStatic(ownerId, itemId, int.Parse(bindings.BindString(s_delta)));
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        return Bind(bindings).WeighAgainstGoal(ws, bindings, resources, goal);
    }

    public override Effect ExecuteEffect(WorldState ws, Townie actor, BoundBindingCollection bindings, FeatureResources resources)
    {
        return Bind(bindings).ExecuteEffect(ws, actor, bindings, resources);
    }
}