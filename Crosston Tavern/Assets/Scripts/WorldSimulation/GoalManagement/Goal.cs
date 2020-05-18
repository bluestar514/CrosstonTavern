using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Goal 
{
    public string name;

    public State state;

    public float priority;

    public int priTimer;
    int priTimerCurrent;

    //public List<Goal> parentGoals = new List<Goal>();
    public List<BoundAction> enablingActions = new List<BoundAction>();

    public Goal(State state, float priority)
    {
        this.state = state;
        this.priority = priority;

        name = state.ToString() + ":" + priority;
    }

    public Goal(State state, float priority, List<BoundAction> enabledActions):this(state, priority)
    {
        //parentGoals.AddRange(parent);
        enablingActions.AddRange(enabledActions);
    }



    public bool OperatingOnSameState(Goal other)
    {
        if (other.state.GetType() != this.state.GetType()) return false;

        if (state is StateInventory) {
            StateInventory invState = (StateInventory)state;
            StateInventory invOther = (StateInventory)other.state;

            if (invState.ownerId != invOther.ownerId) return false;
            if (invState.itemId != invOther.itemId) return false;


            return true;
        }

        if (state is StateSocial) {
            StateSocial socState = (StateSocial)state;
            StateSocial socOther = (StateSocial)other.state;

            if (socState.targetId != socOther.targetId) return false;
            if (socState.sourceId != socOther.sourceId) return false;
            if (socState.axis != socOther.axis) return false;

            return true;
        }

        if (state is StatePosition) {
            StatePosition moveState = (StatePosition)state;
            StatePosition moveOther = (StatePosition)other.state;

            return moveState.locationId == moveOther.locationId;
        }

        return false;
    }


    public Goal CondenseGoal(Goal other)
    {
        if (!OperatingOnSameState(other)) return null;

        float combinedPriority = (priority + other.priority)*3 / 4;

        List<Goal> combinedParents = new List<Goal>();

        List<BoundAction> combinedActions = new List<BoundAction>();
        combinedActions.AddRange(enablingActions);
        combinedActions.AddRange(other.enablingActions);

        if (state is StateInventory) {
            StateInventory invState = (StateInventory)state;
            StateInventory invOther = (StateInventory)other.state;

            int min = Mathf.Max(invState.min, invOther.min);
            int max = Mathf.Min(invState.max, invOther.max);

            if (max < min) {
                if (priority >= other.priority) {
                    return new Goal(state, priority * 2, enablingActions);
                } else {
                    return new Goal(other.state, other.priority * 2, other.enablingActions);
                }
            }

            return new Goal(new StateInventory(invState.ownerId, invState.itemId, min, max),
                                combinedPriority, combinedActions);
        }

        if (state is StateSocial) {
            StateSocial socState = (StateSocial)state;
            StateSocial socOther = (StateSocial)other.state;

            int deltaMin = Mathf.Max(socState.min, socOther.min);
            int deltaMax = Mathf.Min(socState.max, socOther.max);

            if (deltaMax < deltaMin) {
                if (priority >= other.priority) {
                    return new Goal(state, priority * 2, enablingActions);
                } else {
                    return new Goal(other.state, other.priority * 2, other.enablingActions);
                }
            }

            return new Goal(new StateSocial(socState.sourceId, socState.sourceId, socState.axis, deltaMin, deltaMax),
                                combinedPriority, combinedActions);
        }

        if (state is StatePosition) {
            return new Goal(state, combinedPriority, combinedActions);
        }

        return null;
    }
}
