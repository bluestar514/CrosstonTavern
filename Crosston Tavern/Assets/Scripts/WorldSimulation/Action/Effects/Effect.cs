using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effect 
{
    public string id;

    public Effect()
    {
        id = ToString();
    }

    public override string ToString()
    {
        return "<Generic Effect>";
    }
}





public class EffectMovement: Effect
{
    public string moverId;
    public string newLocationId;

    public EffectMovement(string moverId, string newLocationId)
    {
        this.moverId = moverId;
        this.newLocationId = newLocationId;

        id = ToString();
    }

    public override string ToString()
    {
        return "<EffectMovement(" + moverId + "," + newLocationId + ")>";
    }
}