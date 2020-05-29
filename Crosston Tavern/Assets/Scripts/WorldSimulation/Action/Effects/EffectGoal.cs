﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectGoal : Effect
{
    public string owner;
    public GoalModule newGoal;

    public EffectGoal(string owner, GoalModule newGoal)
    {
        this.owner = owner;
        this.newGoal = newGoal;
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(this.owner);
        
        List<GM_Precondition> preconditions = new List<GM_Precondition>(from cond in newGoal.preconditions
                                                                        select cond.MakeSpecific(ws));
        List<Goal> goals = new List<Goal>(from goal in newGoal.goals
                                          select goal.MakeSpecific(bindings, resources));

        string name = bindings.BindString(newGoal.name);

        GoalModule goalModule = new GoalModule(preconditions, goals, name);

        if (townie != null && owner == townie.townieInformation.id) {
            townie.gm.AddModule(goalModule);
        }
        return new EffectGoal(owner, goalModule);
    }

    public override string ToString()
    {
        return "<EffectGoal(" + owner + ", " + newGoal + ")>";
    }
}
