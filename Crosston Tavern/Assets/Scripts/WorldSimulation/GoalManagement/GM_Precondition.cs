using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public virtual string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "ERROR";
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
        if (startTime < endTime) return ws.Time <= endTime && ws.Time >= startTime;
        else return ws.Time >= startTime || ws.Time <= endTime;
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        return base.MakeSpecific(ws);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "It was the right time";
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

        WorldTime start = ws.Time + bufferTime;
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
        WorldTime start = new WorldTime(ws.Time);

        WorldTime end = start + length;

        return new GM_Precondition_Time(start, end);
    }
    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "It was the right time";
    }
}

public class GM_Precondition_State: GM_Precondition
{
    public State state;

    public GM_Precondition_State(State state)
    {
        this.state = state;
    }

    public override bool Satisfied(WorldState ws)
    {
        return state.InEffect(ws, new BoundBindingCollection(), new FeatureResources());
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return state.Verbalize(speaker, listener, false);
    }
}

public class GM_Precondition_TopRelationAxis: GM_Precondition
{
    public string owner;
    public string target; 
    public Relationship.Axis axis;
    public int top;
    public bool reverse; //if true, we want to be in the bottom # instead
    public bool want; // whether we want to be in the top # or if we expilicitly don't

    public GM_Precondition_TopRelationAxis(Relationship.Axis axis,  string owner, string target, 
                                            int top = 1, bool reverse = false, bool want = true)
    {
        this.axis = axis;
        this.top = top;
        this.want = want;
        this.owner = owner;
        this.target = target;
        this.reverse = reverse;
    }

    public override bool Satisfied(WorldState ws)
    {
        Relationship rel = ws.GetRelationshipsFor(owner);


        List<KeyValuePair<string, int>> pairs = new List<KeyValuePair<string, int>>();
        foreach (string knownPerson in rel.GetKnownPeople()) {
            pairs.Add(new KeyValuePair<string, int>(knownPerson, rel.Get(knownPerson, axis)));
        }
        if (reverse) {
            pairs = pairs.OrderBy(pair => pair.Value).ToList();
        } else {
            pairs = pairs.OrderBy(pair => -pair.Value).ToList();
        }
        

        int position = pairs.FindIndex(pair => pair.Key == target);


        return (position < top) == want;
    }

    public override string ToString()
    {
        if (reverse) {
            return "{GoalModule Precondition: TopRelationAxis - " + 
                        owner + "-" + axis + "->" + target + 
                        " (Bottom " + top + ":"+want+ ")}";
        } else {
            return "{GoalModule Precondition: TopRelationAxis - " + 
                        owner + "-" + axis + "->" + target + 
                        " (Top " + top + ":" + want + ")}";
        }
        
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string owner = Verbalizer.VerbalizeSubject(this.owner, speaker, listener);
        string target = Verbalizer.VerbalizeSubject(this.target, speaker, listener);

        string others = "the most";
        if (top > 1) others = "more than almost anyone else";

        string opinion = "";
        if(axis == Relationship.Axis.friendly) {
            if (reverse) {
                opinion = "dislikes";
            } else {
                opinion = "likes";
            }
        } else {
            if (reverse) {
                opinion = "hates";
            } else {
                opinion = "loves";
            }
        }


        return string.Join(" ", new List<string>() { owner, opinion, target, others, "." });
    }
}