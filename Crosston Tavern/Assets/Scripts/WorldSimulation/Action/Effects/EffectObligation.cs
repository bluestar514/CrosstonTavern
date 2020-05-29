using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectObligation : Effect
{
    public string owner;
    public Obligation obligation;

    public EffectObligation(string owner, Obligation obligation)
    {
        this.owner = owner;
        this.obligation = obligation;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(this.owner);


        string obName = bindings.BindString(string.Join(",", resources.BindString(obligation.Name)));

        GoalModule newGoal = obligation.gMod;

        List<GM_Precondition> preconditions = new List<GM_Precondition>(from cond in newGoal.preconditions
                                                                        select cond.MakeSpecific(ws));
        List<Goal> goals = new List<Goal>(from goal in newGoal.goals
                                          select goal.MakeSpecific(bindings, resources));

        string name = bindings.BindString(newGoal.name);

        GoalModule goalModule = new GoalModule(preconditions, goals, name);

        Obligation ob = new Obligation(obName, obligation.Start, obligation.End, obligation.Blocking, goalModule);

        ws.registry.GetPerson(owner).schedule.Add(ob);

        return new EffectObligation(owner, ob);
    }
}
