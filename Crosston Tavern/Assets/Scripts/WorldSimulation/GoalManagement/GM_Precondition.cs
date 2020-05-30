using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Precondition
{
    public virtual bool Satisfied(WorldState ws)
    {
        return true;
    }

    public virtual GM_Precondition MakeSpecific(WorldState ws)
    {
        return this;
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
        if (startTime < endTime) return ws.time <= endTime && ws.time >= startTime;
        else return ws.time >= startTime || ws.time <= endTime;
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        return base.MakeSpecific(ws);
    }

}


/// <summary>
/// This should always be reworked into a GM_Precondition_Time object. 
/// This exists just for generic action definitions.
/// </summary>
public class GM_Precondition_NextFreeTime: GM_Precondition
{
    public List<string> participants;
    public WorldTime bufferTime;
    public WorldTime length;

    public GM_Precondition_NextFreeTime(List<string> participants, WorldTime bufferTime, WorldTime length)
    {
        this.participants = participants;
        this.bufferTime = bufferTime;
        this.length = length;
    }

    public override bool Satisfied(WorldState ws)
    {
        return MakeSpecific(ws).Satisfied(ws);
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        //List<TimeObligation> obligations = new List<TimeObligation>();

        //foreach(string participant in participants) {
        //    List<TimeObligation> ob = ws.registry.GetPerson(participant).timeObligations;

        //    obligations = CondenseTimeObligations(obligations, ob);
        //}

        //WorldTime minTime = ws.time + bufferTime;

        //foreach(TimeObligation obligation in obligations) {
        //    if (obligation.end > minTime) return new GM_Precondition_Time(obligation.end, obligation.end + length);
        //}

        WorldTime start = ws.time + bufferTime;
        WorldTime end = start + length;

        return new GM_Precondition_Time(start, end);
    }

    List<TimeObligation> CondenseTimeObligations(List<TimeObligation> a, List<TimeObligation> b)
    {
        if (a.Count == 0) return b;
        if (b.Count == 0) return a;

        List<TimeObligation> condensed = new List<TimeObligation>();

        int i = 0;
        int j = 0;
        while(i< a.Count) {
            TimeObligation obA = a[i];
            WorldTime aStart = obA.start;
            WorldTime aEnd = obA.end;
            while(j<b.Count) {
                TimeObligation obB = b[j];
                WorldTime bStart = obB.start;
                WorldTime bEnd = obB.end;

                if (obB.HappensBefore(obA)) {
                    condensed.Add(obB);
                    j++;
                    continue;
                }
                if (obA.HappensBefore(obB)) {
                    condensed.Add(obA);
                    i++;
                    continue;
                }

                if (obA.Overlapping(obB)) {
                    WorldTime start;
                    WorldTime end;
                    if (aStart <= bStart) start = aStart;
                    else start = bStart;
                    if (aEnd >= bEnd) end = aEnd;
                    else end = bEnd;

                    obA = new TimeObligation(start, end);

                    j++;
                    continue;
                }
            }
        }

        return condensed;
    }
}

public class GM_Precondition_Now: GM_Precondition
{
    public WorldTime length;

    public GM_Precondition_Now(WorldTime length)
    {
        this.length = length;
    }

    public override bool Satisfied(WorldState ws)
    {
        return MakeSpecific(ws).Satisfied(ws);
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        WorldTime start = new WorldTime(ws.time);

        WorldTime end = start + length;

        return new GM_Precondition_Time(start, end);
    }
}