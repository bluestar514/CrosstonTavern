using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectResourceChange : Effect
{
    public string ownerId;
    public string resourceTag;
    public string potentialBinding;
    public bool add;

    public EffectResourceChange(string ownerId, string resourceTag, string potentialBinding, bool add)
    {
        this.resourceTag = resourceTag;
        this.potentialBinding = potentialBinding;
        this.add = add;
        this.ownerId = ownerId;
    }

    public override string ToString()
    {
        return "<EffectResourceChange(" + resourceTag+":"+potentialBinding+"-"+add+">";
    }
    public override Effect Combine(Effect other)
    {
        if (other is EffectResourceChange) {
            EffectResourceChange otherChange = (EffectResourceChange)other;

            if (ownerId == otherChange.ownerId &&
                resourceTag == otherChange.resourceTag &&
                potentialBinding == otherChange.potentialBinding &&
                add == otherChange.add) {
                return this;
            }
        }

        return null;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie townie, BoundBindingCollection bindings, FeatureResources resources)
    {
        string ownerId = bindings.BindString(this.ownerId);
        string itemid = bindings.BindString(potentialBinding);

        List<string> items = resources.BindString(itemid);
        itemid = items[Mathf.FloorToInt(UnityEngine.Random.value * items.Count)];

        Feature owner = ws.map.GetFeature(ownerId);

        if (add) {
            owner.relevantResources.Add(resourceTag, itemid);
        } else {
            owner.relevantResources.Remove(resourceTag, itemid);
        }


        Effect realizedEffect = new EffectResourceChange(ownerId, resourceTag, itemid, add);

        return realizedEffect;


    }

    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {

        if( add &&
            goal is GoalState goalState && 
            goalState.state is StateInventory stateInv &&
            stateInv.itemId == potentialBinding)
                return .75f;
        else
                return 0;
    }
}
