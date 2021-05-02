using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSkill : State
{
    public string ownerId;
    public string skillId;
    public float min;
    public float max;

    public StateSkill(string ownerId, string skillId, float min, float max)
    {
        this.ownerId = ownerId;
        this.skillId = skillId;
        this.min = min;
        this.max = max;

        id = ToString();
    }

    public override string ToString()
    {
        return "<StateSkill(" + ownerId + "-" + skillId + ":{" + min + "~" + max + "})>";
    }
    public override bool InEffect(WorldState ws, BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string skillName = bindings.BindString(skillId);

        Skill skill = ws.GetSkillFor(owner);
        float value = skill.GetSkillLevel(skillName);

        return value <= max && value >= min;
    }

    public override State Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        string owner = bindings.BindString(ownerId);
        string skillName = bindings.BindString(skillId);

        return new StateSkill(owner, skillName, min, max);
    }

    public override List<State> Combine(State state)
    {
        if (!(state is StateSkill)) return new List<State>() { this, state };
        StateSkill stateSkill = (StateSkill)state;

        if (stateSkill.ownerId != ownerId ||
            stateSkill.skillId != skillId) return new List<State>() { this, state };

        float min = Mathf.Max(this.min, stateSkill.min);
        float max = Mathf.Min(this.max, stateSkill.max);

        if (min > max) return new List<State>() { this, state };

        return new List<State>() {
            new StateSkill(ownerId, skillId, min, max)
        };
    }


    public override string Verbalize(string speakerId, string listenerId, bool goal, bool futureTense = false)
    {

        string owner = ownerId;
        if (speakerId == ownerId) owner = "I";
        if (listenerId == ownerId) owner = "you";

        owner = VerbalizationDictionary.Replace(owner);
        string skillLevel = SkillDictionary.Replace(skillId, (int)min);

        if (goal) {
            if(owner == "I")
                return "to become " + skillLevel; 
            else
                return owner + " to become " + skillLevel; 
        } else {
            if (owner == "you")
                return owner + " are " + skillLevel;
            else
                return owner + " am " + skillLevel;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is StateSkill state) {
            return ownerId == state.ownerId &&
                    skillId == state.skillId;
        } else return false;
    }

    public override int GetHashCode()
    {
        int hashCode = -483002654;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ownerId);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(skillId);
        return hashCode;
    }
}
