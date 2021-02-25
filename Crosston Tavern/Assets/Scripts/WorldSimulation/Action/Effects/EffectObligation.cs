using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectObligation : Effect
{
    public string owner;
    public Obligation obligation;

    public EffectObligation(string owner, Obligation obligation, VerbilizationEffect verbilizationEffect = null)
    {
        this.owner = owner;
        this.obligation = obligation;
        verbalization = verbilizationEffect;
    }


    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(this.owner);


        string obName = string.Join(",", resources.BindString(bindings.BindString(obligation.Name)));

        GoalModule newGoal = obligation.gMod;

        List<GM_Precondition> preconditions = new List<GM_Precondition>(from cond in newGoal.preconditions
                                                                        select cond.MakeSpecific(ws));
        List<Goal> goals = new List<Goal>(from goal in newGoal.goals
                                          select goal.MakeSpecific(bindings, resources));

        string name = bindings.BindString(newGoal.name);

        GoalModule goalModule = new GoalModule(preconditions, goals, name);

        Obligation ob = new Obligation(obName, obligation.Start, obligation.End, obligation.Blocking, goalModule);

        ws.map.GetPerson(owner).schedule.Add(ob);

        return new EffectObligation(owner, ob, verbalization);
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        //Debug.Log("TO DO: Decide how the weighing of Obligation Effects should be done!");

        return base.WeighAgainstGoal(ws, bindings, resources, goal);
    }
}


public class EffectObligationNow: Effect
{
    public string owner;
    public string obligationName;
    public WorldTime length;
    public bool blocking;
    public GoalModule goalModule;

    public EffectObligationNow(string owner, string obligationName, WorldTime length, bool blocking, 
        GoalModule goalModule, VerbilizationEffect verbilizationEffect = null)
    {
        this.owner = owner;
        this.length = length;
        this.blocking = blocking;
        this.goalModule = goalModule;
        this.obligationName = obligationName;
        verbalization = verbilizationEffect;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        WorldTime start = new WorldTime(ws.Time);
        WorldTime end = start + length;

        return new EffectObligation(owner, 
                            new Obligation(obligationName, start, end, blocking, goalModule), verbalization)
            .ExecuteEffect(ws, townie, bindings, resources);
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        //Debug.Log("TO DO: Decide how the weighing of Obligation Effects should be done!");

        return base.WeighAgainstGoal(ws, bindings, resources, goal);
    }
}