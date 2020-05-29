using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GoalModule
{
    public string name;


    public List<GM_Precondition> preconditions;
    public List<Goal> goals;

    public GoalModule(List<GM_Precondition> preconditions, List<Goal> goals, string name = "GENERIC GOAL")
    {
        this.preconditions = preconditions;
        this.goals = goals;
        this.name = name;
    }

    public virtual bool Precondtion(WorldState ws) {
        if (preconditions.Any(p => !p.Satisfied(ws))) return false;
        else return true;
    }

    

}