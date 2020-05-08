using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Goal 
{
    public string name;

    public Effect state;
    float priorityBase;

    public float priority;

    float priModifier;
    public int priTimer;
    int priTimerCurrent;

    //public List<Goal> parentGoals = new List<Goal>();
    public List<BoundAction> enablingActions = new List<BoundAction>();

    public Goal(Effect state, float priority, float priModifier)
    {
        this.state = state;
        this.priority = priority;
        priorityBase = priority;
        this.priModifier = priModifier;

        name = state.ToString() + ":" + priority;
    }

    public Goal(Effect state, float priority, float priModifier,  List<BoundAction> enabledActions):this(state, priority, priModifier)
    {
        //parentGoals.AddRange(parent);
        enablingActions.AddRange(enabledActions);
    }



    public bool OperatingOnSameState(Goal other)
    {
        if (other.state.GetType() != this.state.GetType()) return false;

        //if(state is EffectInvChange) {
        //    EffectInvChange invState = (EffectInvChange)state;
        //    EffectInvChange invOther = (EffectInvChange)other.state;

        //    if (invState.InvOwner != invOther.InvOwner) return false;
        //    if (!(invState.ItemId.All(invOther.ItemId.Contains) &&
        //        invState.ItemId.Count == invOther.ItemId.Count)) return false;


        //    //float deltaAverageState = (invState.DeltaMin + invState.DeltaMax) / 2f;
        //    //float deltaAverageOther = (invState.DeltaMin + invState.DeltaMax) / 2f;
        //    //if (!((deltaAverageState >= 0 && deltaAverageOther >= 0) ||
        //    //    (deltaAverageState <= 0 && deltaAverageOther <= 0))) return false;

        //    return true;
        //}

        //if(state is EffectSocialChange) {
        //    EffectSocialChange socState = (EffectSocialChange)state;
        //    EffectSocialChange socOther = (EffectSocialChange)other.state;

        //    if (socState.TargetId != socOther.TargetId) return false;
        //    if (socState.SourceId != socOther.SourceId) return false;
        //    if (socState.RelationType != socOther.RelationType) return false;

        //    return true;
        //}

        //if(state is EffectMove) {
        //    EffectMove moveState = (EffectMove)state;
        //    EffectMove moveOther = (EffectMove)other.state;

        //    return moveState.TargetLocation == moveOther.TargetLocation;
        //}

        return false;
    }


    public Goal CondenseGoal(Goal other)
    {
        if (!OperatingOnSameState(other)) return null;

        float combinedPriority = priority + other.priority;
        float combinedMod = priModifier + other.priModifier;

        //List<Goal> combinedParents = new List<Goal>();
        //combinedParents.AddRange(parentGoals);
        //combinedParents.AddRange(other.parentGoals);

        List<BoundAction> combinedActions = new List<BoundAction>();
        combinedActions.AddRange(enablingActions);
        combinedActions.AddRange(other.enablingActions);

        //if (state is EffectInvChange) {
        //    EffectInvChange invState = (EffectInvChange)state;
        //    EffectInvChange invOther = (EffectInvChange)other.state;

        //    int deltaMin = Mathf.Max(invState.DeltaMin, invOther.DeltaMin);
        //    int deltaMax = Mathf.Min(invState.DeltaMax, invOther.DeltaMax);

        //    if(deltaMax < deltaMin) {
        //        if (priority >= other.priority) {
        //            return new Goal(state, priority * 2, priModifier,  enablingActions);
        //        } else {
        //            return new Goal(other.state, other.priority * 2, other.priModifier,  other.enablingActions);
        //        }
        //    }

        //    return new Goal(new EffectInvChange(deltaMin, deltaMax, invState.InvOwner, invState.ItemId), 
        //                        combinedPriority, combinedMod, combinedActions);
        //}

        //if (state is EffectSocialChange) {
        //    EffectSocialChange socState = (EffectSocialChange)state;
        //    EffectSocialChange socOther = (EffectSocialChange)other.state;

        //    int deltaMin = Mathf.Max(socState.DeltaMin, socOther.DeltaMin);
        //    int deltaMax = Mathf.Min(socState.DeltaMax, socOther.DeltaMax);

        //    if (deltaMax < deltaMin) {
        //        if (priority >= other.priority) {
        //            return new Goal(state, priority * 2, priModifier, enablingActions);
        //        } else {
        //            return new Goal(other.state, other.priority * 2, other.priModifier,  other.enablingActions);
        //        }
        //    }

        //    return new Goal(new EffectSocialChange(deltaMin, deltaMax, socState.SourceId, socState.TargetId, socState.RelationType),
        //                        combinedPriority, combinedMod, combinedActions);
        //}

        //if (state is EffectMove) {
        //    return new Goal(state, combinedPriority, combinedMod, combinedActions);
        //}

        return null;
    }
}
