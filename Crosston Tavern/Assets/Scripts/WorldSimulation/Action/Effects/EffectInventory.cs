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

public class EffectInventoryVariable<T> : EffectInventory
{
    public List<string> itemIds= new List<string>();
    public T min;
    public T max;

    public EffectInventoryVariable(string ownerId, List<string> itemIds, T min, T max)
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

public class EffectInventoryRange<T> : EffectInventory
{
    public string itemId;
    public T min;
    public T max;

    public EffectInventoryRange(string ownerId, string itemId, T min, T max)
    {
        this.ownerId = ownerId;
        this.itemId = itemId;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return base.ToString() + ",{" + min + "~" + max + "}," +  itemId+ ")>";
    }
}

public class EffectInventoryStatic<T> : EffectInventory
{
    public string itemId;
    public T delta;

    public EffectInventoryStatic(string ownerId, string itemId, T delta)
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