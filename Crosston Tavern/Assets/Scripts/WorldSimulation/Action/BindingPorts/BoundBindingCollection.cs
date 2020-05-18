﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundBindingCollection 
{
    public List<BoundBindingPort> bindings;

    public int Count { get => bindings.Count;}

    public BoundBindingCollection(List<BoundBindingPort> bindings)
    {
        this.bindings = bindings;
    }

    public string BindString(string str)
    {
        if (bindings == null) return str;

        foreach (BoundBindingPort binding in bindings) {
            str = str.Replace("#" + binding.tag + "#", binding.Value);
            if (binding is BoundPortInventoryItem) {
                BoundPortInventoryItem item = (BoundPortInventoryItem)binding;
                str = str.Replace("#" + binding.tag + ".count#", item.itemCount.ToString());
            }
            if (binding is BoundPortStockItem) {
                BoundPortStockItem stock = (BoundPortStockItem)binding;
                str = str.Replace("#" + binding.tag + ".cost#", stock.itemCost.ToString());
            }
        }

        return str;
    }
}