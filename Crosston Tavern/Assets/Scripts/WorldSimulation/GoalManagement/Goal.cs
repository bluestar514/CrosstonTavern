using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Goal 
{
    public string name;

    public MicroEffect state;
    float priorityBase;

    public float priority;

    float priModifier;
    public int priTimer;
    int priTimerCurrent;

    public List<Goal> parentGoals = new List<Goal>();
    public List<BoundAction> enablingActions = new List<BoundAction>();

    public Goal(MicroEffect state, float priority, float priModifier)
    {
        this.state = state;
        this.priority = priority;
        priorityBase = priority;
        this.priModifier = priModifier;

        name = state.ToString() + ":" + priority;
    }

    public Goal(MicroEffect state, float priority, float priModifier, List<Goal> parent, List<BoundAction> enabledActions):this(state, priority, priModifier)
    {
        parentGoals.AddRange(parent);
        enablingActions.AddRange(enabledActions);
    }



    public bool OperatingOnSameState(Goal other)
    {
        if (other.state.GetType() != this.state.GetType()) return false;

        if(state is InvChange) {
            InvChange invState = (InvChange)state;
            InvChange invOther = (InvChange)other.state;

            if (invState.InvOwner != invOther.InvOwner) return false;
            if (!(invState.ItemId.All(invOther.ItemId.Contains) &&
                invState.ItemId.Count == invOther.ItemId.Count)) return false;


            //float deltaAverageState = (invState.DeltaMin + invState.DeltaMax) / 2f;
            //float deltaAverageOther = (invState.DeltaMin + invState.DeltaMax) / 2f;
            //if (!((deltaAverageState >= 0 && deltaAverageOther >= 0) ||
            //    (deltaAverageState <= 0 && deltaAverageOther <= 0))) return false;

            return true;
        }

        if(state is SocialChange) {
            SocialChange socState = (SocialChange)state;
            SocialChange socOther = (SocialChange)other.state;

            if (socState.TargetId != socOther.TargetId) return false;
            if (socState.SourceId != socOther.SourceId) return false;

            return true;
        }

        if(state is Move) {
            Move moveState = (Move)state;
            Move moveOther = (Move)other.state;

            return moveState.TargetLocation == moveOther.TargetLocation;
        }

        return false;
    }


    public Goal CondenseGoal(Goal other)
    {
        if (!OperatingOnSameState(other)) return null;

        float combinedPriority = priority + other.priority;
        float combinedMod = priModifier + other.priModifier;

        List<Goal> combinedParents = new List<Goal>();
        combinedParents.AddRange(parentGoals);
        combinedParents.AddRange(other.parentGoals);

        List<BoundAction> combinedActions = new List<BoundAction>();
        combinedActions.AddRange(enablingActions);
        combinedActions.AddRange(other.enablingActions);

        if (state is InvChange) {
            InvChange invState = (InvChange)state;
            InvChange invOther = (InvChange)other.state;

            int deltaMin = Mathf.Max(invState.DeltaMin, invOther.DeltaMin);
            int deltaMax = Mathf.Min(invState.DeltaMax, invOther.DeltaMax);

            if(deltaMax < deltaMin) {
                if (priority >= other.priority) {
                    return new Goal(state, priority * 2, priModifier, parentGoals, enablingActions);
                } else {
                    return new Goal(other.state, other.priority * 2, other.priModifier, other.parentGoals, other.enablingActions);
                }
            }

            return new Goal(new InvChange(deltaMin, deltaMax, invState.InvOwner, invState.ItemId), 
                                combinedPriority, combinedMod, combinedParents, combinedActions);
        }

        if (state is SocialChange) {
            SocialChange socState = (SocialChange)state;
            SocialChange socOther = (SocialChange)other.state;

            int deltaMin = Mathf.Max(socState.DeltaMin, socOther.DeltaMin);
            int deltaMax = Mathf.Min(socState.DeltaMax, socOther.DeltaMax);

            if (deltaMax < deltaMin) {
                if (priority >= other.priority) {
                    return new Goal(state, priority * 2, priModifier, parentGoals, enablingActions);
                } else {
                    return new Goal(other.state, other.priority * 2, other.priModifier, other.parentGoals, other.enablingActions);
                }
            }

            return new Goal(new SocialChange(deltaMin, deltaMax, socState.SourceId, socState.TargetId),
                                combinedPriority, combinedMod, combinedParents, combinedActions);
        }

        if (state is Move) {
            return new Goal(state, combinedPriority, combinedMod, combinedParents, combinedActions);
        }

        return null;
    }
}
