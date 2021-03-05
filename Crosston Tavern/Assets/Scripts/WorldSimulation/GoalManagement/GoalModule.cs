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

    public string reason;

    public int timer;

    public GoalModule(List<GM_Precondition> preconditions, List<Goal> goals, 
        string reason="I just do", string name = "GENERIC GOAL",  int timer = -1)
    {
        this.preconditions = preconditions;
        this.goals = goals;
        this.reason = reason;
        this.name = name;
        this.timer = timer;
    }

    public void DecrementTimer()
    {
        if (timer > 0) timer--;
    }

    public virtual bool Precondtion(WorldState ws) {
        if (preconditions.Any(p => !p.Satisfied(ws))) return false;
        else return true;
    }

    public override string ToString()
    {
        return name;
    }

}