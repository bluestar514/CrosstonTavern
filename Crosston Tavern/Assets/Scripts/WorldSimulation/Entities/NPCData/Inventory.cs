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

    protected Inventory(string owner, StringIntDictionary inv)
    {
        this.owner = owner;
        inventory = new StringIntDictionary();
        inventory.CopyFrom(inv);
    }

    public Inventory Copy(bool perfect)
    {
        if (perfect) return new Inventory(owner, inventory);
        else return new Inventory(owner);
    }

    public int GetInventoryCount(string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            return inventory[itemId];
        } else {
            return 0;
        }
    }

    public int GetInventoryCount(List<string> itemIds)
    {
        int count = 0;
        foreach (string itemId in itemIds) {

            if (inventory.ContainsKey(itemId)) {
                count+= inventory[itemId];
            }
        }

        return count;
    }

    public void ChangeInventoryContents(int num, string itemId)
    {
        if (inventory.ContainsKey(itemId)) {
            inventory[itemId] += num;
        } else {
            inventory.Add(itemId, num);
        }
    }

    public IEnumerable<string> GetItemList()
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
