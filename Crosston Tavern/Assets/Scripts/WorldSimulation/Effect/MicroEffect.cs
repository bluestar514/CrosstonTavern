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

    public virtual MicroEffect SpecifyEffect()
    {
        return new MicroEffect();
    }

    public virtual bool GoalComplete(WorldState ws, Person actor)
    {
        return false;
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
    public string InvOwner { get; private set; }
    public List<string> ItemId { get; private set; }

    public InvChange(int deltaMin, int deltaMax, string invOwner, List<string> itemId)
    {
        DeltaMin = deltaMin;
        DeltaMax = deltaMax;
        InvOwner = invOwner;
        ItemId = itemId;

        id = ToString();
    }

    public override MicroEffect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new InvChange(DeltaMin, DeltaMax,BindId(InvOwner, resources), BindId(ItemId, resources));
    }

    public override MicroEffect SpecifyEffect()
    {
        int randNum = UnityEngine.Random.Range(DeltaMin, DeltaMax + 1);
        int randItem = Mathf.FloorToInt(UnityEngine.Random.value * ItemId.Count);

        return new InvChange(randNum, randNum, InvOwner, new List<string>() { ItemId[randItem] });
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        Registry registry = ws.registry;

        StringIntDictionary inv = registry.GetPerson(InvOwner).inventory;

        int count = 0;
        foreach (string item in ItemId) {
            if (inv.ContainsKey(item)) count += inv[item];
        }


        return count >= DeltaMin && count <= DeltaMax;
    }
    public override string ToString()
    {
        return "<InvChange("+InvOwner+", {" + DeltaMin + "~" + DeltaMax + "}, {" + String.Join(",", ItemId) + "} )>";
    }
}

public class Move: MicroEffect
{
    private string targetLocation = "#connectedLocation#";

    public string TargetLocation { get => targetLocation; private set => targetLocation = value; }

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


    public override MicroEffect SpecifyEffect()
    {
        return new Move(TargetLocation);
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        return actor.location == targetLocation;
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

    public override MicroEffect SpecifyEffect()
    {
        int randNum = UnityEngine.Random.Range(DeltaMin, DeltaMax + 1);

        return new SocialChange(randNum, randNum, SourceId, TargetId);
    }

    public override bool GoalComplete(WorldState ws, Person actor)
    {
        Registry registry = ws.registry;

        Person person = registry.GetPerson(SourceId);
        float val = person.GetRelationshipValue(TargetId);

        return  val >= DeltaMin && val <= DeltaMax;
    }

    public override string ToString()
    {
        return "<SocialChange({" + DeltaMin + "~" + DeltaMax + "}, " + SourceId + "->"+TargetId + ")>";
    }
}