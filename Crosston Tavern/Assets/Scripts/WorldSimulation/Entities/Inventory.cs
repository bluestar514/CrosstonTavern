using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    string owner;

    [SerializeField]
    StringIntDictionary inventory = new StringIntDictionary();

    public Inventory(string owner)
    {
        this.owner = owner;
    }

    public int GetInventoryCount(string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            return inventory[itemId];
        } else {
            return 0;
        }
    }
    public void ChangeInventoryContents(int num, string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            inventory[itemId] += num;
        } else {
            inventory.Add(itemId, num);
        }
    }

    public IEnumerable GetItemList()
    {
        return inventory.Keys;
    }

    public string Print()
    {
        string body = "";

        foreach (string key in inventory.Keys) {
            body += "\t" + key + ": " + inventory[key] + "\n";
        }

        return body;
    }

    public override string ToString()
    {
        return owner + "'s inventory";
    }
}
