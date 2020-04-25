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

    public GoalModule(List<GM_Precondition> preconditions, List<Goal> goals)
    {
        this.preconditions = preconditions;
        this.goals = goals;

        name = "GENERIC GOAL";
    }

    public virtual bool Precondtion(WorldState ws) {
        if (preconditions.Any(p => !p.Satisfied(ws))) return false;
        else return true;
    }

    

}