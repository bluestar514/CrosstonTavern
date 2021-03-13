using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Goal
{
    public string name;

    public float priority;

    public int priTimer;
    protected int priTimerCurrent;

    public int timeOut = PERMINATE;

    public static int PERMINATE = -1;

    public List<string> parentGoals = new List<string>();
    protected List<Goal> parentGoalObj = new List<Goal>();

    
    /// These are the actions completing this goal unlocks,
    /// not the actions which complete this goal.
    /// In other words, these are the actions that the character wanted to do which prompted the creation on this goal.
    public List<BoundAction> unlockedActionsOnGoalCompletion = new List<BoundAction>();

    public virtual List<Goal> Combine(Goal goal)
    {
        return new List<Goal>() { this, goal};
    }

    public virtual Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return this;
    }

    public void AddParentGoal(Goal goal)
    {
        if (parentGoalObj.Contains(goal))
            return;

        parentGoalObj.Add(goal);
        parentGoals.Add(goal.name);
    }
    public List<Goal> GetParentGoals()
    {
        return parentGoalObj;
    }

    public void AddUnlockedAction(BoundAction action)
    {
        if (unlockedActionsOnGoalCompletion.Contains(action)) return;

        unlockedActionsOnGoalCompletion.Add(action);
    }

    public override bool Equals(object obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 363513814 + EqualityComparer<string>.Default.GetHashCode(name);
    }
}

[System.Serializable]
public class GoalState : Goal
{

    public State state;
    public GoalState(State state, float priority)
    {
        this.state = state;
        this.priority = priority;

        name = state.ToString();
    }

    public GoalState(State state, float priority, int timeOut): this(state, priority)
    {
        this.timeOut = timeOut; 
    }

    public override string ToString()
    {
        return state.ToString() + ":" + priority; 
    }

    public override List<Goal> Combine(Goal goal)
    {
        if(goal is GoalState) {
            return new List<Goal>(Combine((GoalState)goal));
        }

        return base.Combine(goal);
    }

    List<GoalState> Combine(GoalState goal)
    {
        List<State> states = this.state.Combine(goal.state);
        if (states.Count == 1) {
            GoalState newGoal = new GoalState(states[0], this.priority + goal.priority, Mathf.Min(this.timeOut, goal.timeOut));
            newGoal.unlockedActionsOnGoalCompletion.AddRange(unlockedActionsOnGoalCompletion);
            newGoal.unlockedActionsOnGoalCompletion.AddRange(goal.unlockedActionsOnGoalCompletion);

            newGoal.parentGoals.AddRange(parentGoals);
            newGoal.parentGoals.AddRange(goal.parentGoals);

            newGoal.parentGoalObj.AddRange(GetParentGoals());
            newGoal.parentGoalObj.AddRange(goal.GetParentGoals());

            return new List<GoalState>() { newGoal };
        } else return new List<GoalState>() { this, goal }; 
    }


    public override Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new GoalState(state.Bind(bindings, resources), priority);
    }


    public override bool Equals(object obj)
    {
        if (obj is GoalState goal) {
            return state.Equals( goal.state );
        
        } else return false;
    }

    public override int GetHashCode()
    {
        return 259708774 + EqualityComparer<State>.Default.GetHashCode(state);
    }
}

public class GoalAction : Goal
{
    public BoundAction action;

    public GoalAction(BoundAction action, float priority)
    {
        this.action = action;
        this.priority = priority;

        name = action.ToString();
    }

    public GoalAction(BoundAction action, float priority, int timeOut) : this(action, priority)
    {
        this.timeOut = timeOut;
    }

    public override List<Goal> Combine(Goal goal)
    {
        if (goal is GoalAction goalAction) {
            BoundAction otherAction = goalAction.action;

            if (action == otherAction) {
                Goal newGoal = new GoalAction(action, this.priority + goal.priority, Mathf.Min(this.timeOut, goal.timeOut));
                return new List<Goal>() { newGoal };
            }
        }
        return base.Combine(goal);
    }

    /// <summary>
    /// I am about 90% sure this shouldn't do anything for the action version of this class
    /// </summary>
    /// <param name="bindings"></param>
    /// <param name="resources"></param>
    /// <returns></returns>
    public override Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return base.MakeSpecific(bindings, resources);
    }

    public override string ToString()
    {
        return action.ToString() + ":" + priority;
    }

    public override bool Equals(object obj)
    {
        if (obj is GoalAction goal) {
            return action.Equals(goal.action);

        } else return false;
    }

    public override int GetHashCode()
    {
        return -1387187753 + EqualityComparer<BoundAction>.Default.GetHashCode(action);
    }
}

