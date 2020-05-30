using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
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
            if ((ob.Start <= start && start <= ob.End)||
                (start <= ob.Start && ob.Start <= end)){
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

[System.Serializable]
public class Obligation
{
    [SerializeField] private string name;

    public GoalModule gMod;

    
    [SerializeField] private bool blocking;
    [SerializeField] private WorldTime end;
    [SerializeField] private WorldTime start;

    public WorldTime Start { get => start; private set => start = value; }
    public WorldTime End { get => end; private set => end = value; }
    public bool Blocking { get => blocking; private set => blocking = value; }
    public string Name { get => name; private set => name = value; }

    public Obligation(string name, WorldTime start, WorldTime end, bool blocking, GoalModule gMod = null)
    {
        this.Name = name;
        this.Start = start;
        this.End = end;
        this.Blocking = blocking;
        this.gMod = gMod;
    }

    public override string ToString()
    {
        return "(Obligation:"+name+")";
    }
}