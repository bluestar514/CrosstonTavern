using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundBindingPort 
{
    public string tag;
    public virtual string Value { get; private set; }

    public BoundBindingPort(string tag)
    {
        this.tag = tag;
    }

    public BoundBindingPort(string tag, string value)
    {
        this.tag = tag;
        Value = value;
    }

    public virtual void BindResource(FeatureResources resources)
    {
        Value = resources.PickBoundString(Value);
    } 
}

public class BoundPortInventoryItem: BoundBindingPort
{
    public string itemId;
    public int itemCount;

    public override string Value => itemId;

    public BoundPortInventoryItem(string tag, string itemId, int itemCount): base(tag)
    {
        this.itemId = itemId;
        this.itemCount = itemCount;
    }

    public override string ToString()
    {
        return "<" + tag + ":" + itemId + "(" + itemCount + ")>";
    }

    public override void BindResource(FeatureResources resources)
    {
        itemId = resources.PickBoundString(itemId);
    }
}

public class BoundPortStockItem: BoundPortInventoryItem
{
    public int itemCost;

    public BoundPortStockItem(string tag, string itemId, int itemCount, int itemCost): base(tag, itemId, itemCount)
    {
        this.itemCost = itemCost;
    }
    public override string ToString()
    {
        return "<" + tag + ":" + itemId + "(" + itemCount+","+itemCost + ")>";
    }
}

public class BoundPortEntity: BoundBindingPort
{
    public string participantId;

    public override string Value => participantId;

    public BoundPortEntity(string tag, string participantId): base(tag)
    {
        this.participantId = participantId;
    }

    public override string ToString()
    {
        return "<" + tag + ":" + participantId + ">";
    }
}
