using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceModifierSkill : ChanceModifier
{
    public StateSkill state;
    public bool positive; //reverses min and max chances

    public ChanceModifierSkill(StateSkill state, bool positive)
    {
        this.state = state;

        this.positive = positive;
    }

    public override float Chance(WorldState ws)
    {

        Feature owner = ws.map.GetFeature(state.ownerId);

        float currentSkillLevel = owner.skill.GetSkillLevel(state.skillId);

        currentSkillLevel = Mathf.Max(0, currentSkillLevel - state.min);
        float max = state.max - state.min;

        currentSkillLevel = currentSkillLevel / max;

        if (!positive) return 1 - currentSkillLevel;
        else return currentSkillLevel;
    }

    public override ChanceModifier MakeBound(BoundBindingCollection bindings, FeatureResources featureResources)
    {
        string owner = bindings.BindString(this.state.ownerId);

        return new ChanceModifierSkill(new StateSkill(owner, state.skillId, state.min, state.max), positive);
    }

    public override List<Goal> MakeGoal(WorldState ws, float priority)
    {

        if (positive) {
            return new List<Goal>() { new Goal(new StateSkill(state.ownerId, state.skillId, state.max, 100), priority) };
        } else {
            return new List<Goal>() { new Goal(new StateSkill(state.ownerId, state.skillId, -100, state.min), priority) };
        }
    }
}
