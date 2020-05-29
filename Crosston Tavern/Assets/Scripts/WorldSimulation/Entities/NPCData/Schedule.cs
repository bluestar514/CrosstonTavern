using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule 
{
    public List<Obligation> obligations;

    public Schedule(List<Obligation> obligations=null)
    {
        if (obligations == null) obligations = new List<Obligation>();

        this.obligations = obligations;
    }

    public bool TimeFree(WorldTime start, WorldTime end, bool onlyBlocking = false)
    {
        foreach(Obligation ob in obligations) {
            if (ob.Start <= start && ob.End >= end) {
                if(ob.Blocking || !onlyBlocking) return false;
            }
        }

        return true;
    }

    public void Add(Obligation ob, bool considerConflicts = true)
    {
        if (considerConflicts && TimeFree(ob.Start, ob.End, ob.Blocking)) {
            obligations.Add(ob);
        }
    }

    public Schedule Copy()
    {
        return new Schedule(new List<Obligation>(obligations));
    }


}


public class Obligation
{
    public GoalModule gMod;

    public WorldTime Start { get; private set; }
    public WorldTime End { get; private set; }
    public bool Blocking { get; private set; }
    public string Name { get; private set; }

    public Obligation(string name, WorldTime start, WorldTime end, bool blocking, GoalModule gMod=null)
    {
        this.Name = name;
        this.Start = start;
        this.End = end;
        this.Blocking = blocking;
        this.gMod = gMod;
    }
}