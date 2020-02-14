using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MicroEffect 
{
    [SerializeField]
    protected string id;

    public MicroEffect()
    {
    }


    public virtual MicroEffect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new MicroEffect();
    }

    public static List<string> BindId(List<string> idList, Dictionary<string, List<string>> resources)
    {
        List<string> newId = new List<string>();
        foreach (string id in idList) {
            string[] split = id.Split(',');
            
            foreach (string item in split) {
                if (item.StartsWith("#") && item.EndsWith("#")
                    && resources.ContainsKey(item.Trim('#'))) {
                    newId.AddRange(resources[item.Trim('#')]);
                } else {
                    newId.Add(item);
                }
            }
        }

        return newId;
    }
    public static string BindId(string item, Dictionary<string, List<string>> resources)
    {
        if (item.StartsWith("#") && item.EndsWith("#")
            && resources.ContainsKey(item.Trim('#'))) {
            return String.Join(",", resources[item.Trim('#')]);
        } else {
            return item;
        }

    }

    public override string ToString()
    {
        return "<GenericMicroEffect>";
    }
}


public class InvChange: MicroEffect
{ 

    public int DeltaMin { get; private set; }
    public int DeltaMax { get; private set; }
    public List<string> ItemId { get; private set; }

    public InvChange(int deltaMin, int deltaMax, List<string> itemId)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        ItemId = itemId;

        id = ToString();
    }

    public override MicroEffect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new InvChange(DeltaMin, DeltaMax, BindId(ItemId, resources));
    }

    public override string ToString()
    {
        return "<InvChange({" + DeltaMin + "~" + DeltaMax + "}, {" + String.Join(",", ItemId) + "} )>";
    }
}

public class Move: MicroEffect
{
    private string targetLocation = "#connectedLocation#";

    public Move(string targetLocation)
    {
        this.targetLocation = targetLocation;
        id = ToString();
    }

    public Move(){
        id = ToString();
    }

    public override MicroEffect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new Move(BindId(targetLocation, resources));
    }

    public override string ToString()
    {
        return "<Move(" + targetLocation + ")>";
    }

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

        id = ToString();
    }

    public override MicroEffect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new SocialChange(DeltaMin, DeltaMax, 
                                BindId(SourceId, resources), 
                                BindId(TargetId, resources));
    }

    public override string ToString()
    {
        return "<SocialChange({" + DeltaMin + "~" + DeltaMax + "}, " + SourceId + "->"+TargetId + ")>";
    }
}