using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Precondition
{
    public virtual bool Satisfied(WorldState ws)
    {
        return true;
    }
}

public class GM_Precondition_Time : GM_Precondition
{
    WorldTime startTime;
    WorldTime endTime;

    public GM_Precondition_Time(WorldTime startTime, WorldTime endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public override bool Satisfied(WorldState ws)
    {
        return ws.time <= endTime && ws.time >= startTime;
    }
}