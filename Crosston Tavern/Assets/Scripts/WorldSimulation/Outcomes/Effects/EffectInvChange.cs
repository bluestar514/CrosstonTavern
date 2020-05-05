using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInvChange : Effect
{

    public int DeltaMin { get; private set; }
    public int DeltaMax { get; private set; }
    public string InvOwner { get; private set; }
    public List<string> ItemId { get; private set; }

    public EffectInvChange(int deltaMin, int deltaMax, string invOwner, List<string> itemId)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;

        InvOwner = invOwner;
        ItemId = itemId;

        id = ToString();
    }

    public override Effect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new EffectInvChange(DeltaMin, DeltaMax, BindId(InvOwner, resources), BindId(ItemId, resources));
    }

    public override Effect SpecifyEffect()
    {
        int randNum = UnityEngine.Random.Range(DeltaMin, DeltaMax + 1);
        int randItem = Mathf.FloorToInt(UnityEngine.Random.value * ItemId.Count);
        Debug.Log(ToString()+" "+ ItemId.Count +" "+randItem);
        return new EffectInvChange(randNum, randNum, InvOwner, new List<string>() { ItemId[randItem] });
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        Registry registry = ws.registry;

        Inventory inv = ws.GetInventory(InvOwner);

        int count = 0;
        foreach (string item in ItemId) {
            count += inv.GetInventoryCount(item);
        }


        return count >= DeltaMin && count <= DeltaMax;
    }
    public override string ToString()
    {
        return "<InvChange(" + InvOwner + ", {" + DeltaMin + "~" + DeltaMax + "}, {" + string.Join(",", ItemId) + "} )>";
    }
}