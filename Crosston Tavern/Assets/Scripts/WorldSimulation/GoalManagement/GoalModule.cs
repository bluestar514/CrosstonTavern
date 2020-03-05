using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalModule
{
    protected Person actor;

    public List<Goal> goals;

    public GoalModule(Person actor, List<Goal> goals)
    {
        this.actor = actor;
        this.goals = goals;
    }

    public virtual bool Precondtion(WorldState ws) {
        return true;
    }

    

}


public class GM_Profession: GoalModule
{
    WorldTime startTime;
    WorldTime endTime;

    public GM_Profession(Person actor, List<Goal> goals, WorldTime startTime, WorldTime endTime) : base(actor, goals)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public override bool Precondtion(WorldState ws)
    {
        return ws.time <= endTime && ws.time >= startTime;
    }
}