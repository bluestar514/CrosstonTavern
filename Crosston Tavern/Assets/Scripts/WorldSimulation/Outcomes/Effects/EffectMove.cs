using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMove : Effect
{
    private string targetLocation = "#connectedLocation#";

    public string TargetLocation { get => targetLocation; private set => targetLocation = value; }

    public EffectMove(string targetLocation)
    {
        this.targetLocation = targetLocation;
        id = ToString();
    }

    public EffectMove()
    {
        id = ToString();
    }

    public override Effect BindEffect(Dictionary<string, List<string>> resources)
    {
        return new EffectMove(BindId(targetLocation, resources));
    }


    public override Effect SpecifyEffect()
    {
        return new EffectMove(TargetLocation);
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
