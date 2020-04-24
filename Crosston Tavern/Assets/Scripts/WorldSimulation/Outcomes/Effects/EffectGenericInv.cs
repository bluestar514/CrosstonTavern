using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGenericInv : Effect
{
    public EffectGenericInv(string deltaMin, string deltaMax, string invOwner, List<string> itemId)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        InvOwner = invOwner;
        ItemId = itemId;
    }

    public string DeltaMin { get; private set; }
    public string DeltaMax { get; private set; }

    public string InvOwner { get; private set; }
    public List<string> ItemId { get; private set; }

    public override Effect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new EffectGenericInv(DeltaMin, DeltaMax, BindId(InvOwner, resources), BindId(ItemId, resources));
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        Debug.LogWarning("Trying to determine if a generic Effect is complete has no meaning");

        return false;
    }

    public override Effect SpecifyEffect()
    {
        Debug.LogWarning("Trying to specify a generic effect has no meaning");

        return this;
    }

    public override string ToString()
    {
        return "<EffectGenericInv(" + InvOwner + ", {" + DeltaMin + "~" + DeltaMax + "}, {" + string.Join(",", ItemId) + "} )>";
    }
}
