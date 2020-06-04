using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingPort {
    public string tag;
}


public class BindingPortInventoryItem: BindingPort
{
    public string owner;

    public BindingPortInventoryItem(string tag, string owner)
    {
        this.tag = tag;
        this.owner = owner;
    }
}

public class BindingPortStockItem : BindingPort
{
    public string shopId;

    public BindingPortStockItem(string tag, string shopId)
    {
        this.tag = tag;
        this.shopId = shopId;
    }
}

public class BindingPortEntity : BindingPort
{
    public ActionRole role;

    public BindingPortEntity(string tag, ActionRole role)
    {
        this.tag = tag;
        this.role = role;
    }
}

public class BindingPortString : BindingPort
{
    public string value;

    public BindingPortString(string tag, string value)
    {
        this.tag = tag;
        this.value = value;
    }
}


public enum ActionRole
{
    initiator,
    recipient, 
    bystander, 
    any,
    location_current,
    location_any
}