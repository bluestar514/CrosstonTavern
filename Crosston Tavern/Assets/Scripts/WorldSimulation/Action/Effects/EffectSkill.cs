using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSkill : Effect
{
    public string ownerId;
    public string skillId;
    public float delta;

    public EffectSkill(string ownerId, string skillId, float delta, VerbilizationEffect verbilizationEffect = null)
    {
        this.ownerId = ownerId;
        this.skillId = skillId;
        this.delta = delta;
        verbalization = verbilizationEffect;
        id = ToString();
    }

    public override string ToString()
    {
        return "<EffectSkill(" + ownerId + "-" + skillId + ":" + delta + ")";
    }


    public override float WeighAgainstGoal(WorldState ws, BoundBindingCollection bindings, FeatureResources resources, Goal goal)
    {
        if(goal is GoalState goalState &&
            goalState.state is StateSkill state) {


            string owner = bindings.BindString(ownerId);
            string skillName = bindings.BindString(skillId);

            string goalOwner = state.ownerId;
            string goalSkill = state.skillId;

            if (owner != goalOwner ||
                skillName != goalSkill) return 0;

            Skill skill = ws.GetSkillFor(owner);
            float value = skill.GetSkillLevel(skillName);

            return CountInRange(value, delta, state.min, state.max);
        }


        return 0;
    }

    public override Effect ExecuteEffect(WorldState ws, Townie actor, BoundBindingCollection bindings, FeatureResources resources)
    {

        string owner = bindings.BindString(ownerId);
        string skillName = bindings.BindString(skillId);

        Skill skill = ws.GetSkillFor(owner);
        skill.Increase(skillName, delta);

        return new EffectSkill(owner, skillName, delta, verbalization);
    }

}
