using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGoal : Effect
{
    public string owner;
    public GoalModule newGoal;

    public EffectGoal(string owner, GoalModule newGoal)
    {
        this.owner = owner;
        this.newGoal = newGoal;
    }


    public override string ToString()
    {
        return "<EffectGoal(" + owner + ", " + newGoal + ")>";
    }
}
