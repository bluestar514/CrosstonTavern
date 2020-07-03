using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStatusEffect : Effect
{
    public string targetId;

    public EntityStatusEffect status;

    public EffectStatusEffect(string targetId, EntityStatusEffect status)
    {
        this.targetId = targetId;
        this.status = status;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        string targetId = bindings.BindString(this.targetId);
        Person target = ws.registry.GetPerson(targetId);


        EntityStatusEffect status = new EntityStatusEffect(this.status);
        List<string> statusTargets = new List<string>();
        foreach (string statusTarget in status.target) {
            statusTargets.Add(bindings.BindString(statusTarget));
        }
        status.target = statusTargets;


        target.statusEffectTable.Add(status);

        return new EffectStatusEffect(targetId, status);
    }

    public override string ToString()
    {
        return "EffectStatusEffect(" + targetId + "," + status + ")>";
    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        return base.WeighAgainstGoal(ws, bindings, resources, goal);
    }
}
