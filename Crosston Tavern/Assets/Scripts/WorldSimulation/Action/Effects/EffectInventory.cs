using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInventory : Effect
{
    public string ownerId;

    public override string ToString()
    {
        return "<EffectInventory("+ownerId;
    }
}

public class EffectInventoryVariable : EffectInventory
{
    public List<string> itemIds= new List<string>();
    public int min;
    public int max;

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
}

public class EffectInventoryStatic : EffectInventory
{
    public string itemId;
    public int delta;

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
}