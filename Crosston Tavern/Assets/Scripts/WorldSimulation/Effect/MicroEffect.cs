using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MicroEffect 
{

    public MicroEffect()
    {
    }

}


public class InvChange: MicroEffect
{ 

    public int DeltaMin { get; private set; }
    public int DeltaMax { get; private set; }
    public string ItemId { get; private set; }

    public InvChange(int deltaMin, int deltaMax, string itemId): base()
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        ItemId = itemId;
    }
}

public class Move: MicroEffect
{

}

public class SocialChange : MicroEffect
{
    public int DeltaMin { get; private set; }
    public int DeltaMax { get; private set; }
    public string SourceId { get; private set; }
    public string TargetId { get; private set; }

    public SocialChange(int deltaMin, int deltaMax, string sourceId, string targetId)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        SourceId = sourceId;
        TargetId = targetId;
    }
}