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

    public int timeOut = PERMINATE;

    public static int PERMINATE = -1;

    public List<string> parentGoals = new List<string>();
    List<Goal> parentGoalObj = new List<Goal>();
    /// <summary>
    /// These are the actions completing this goal unlocks,
    /// not the actions which complete this goal.
    /// In other words, these are the actions that the character wanted to do which prompted the creation on this goal.
    /// </summary>
    public List<BoundAction> unlockedActionsOnGoalCompletion = new List<BoundAction>(); 

    public Goal(State state, float priority)
    {
        this.state = state;
        this.priority = priority;

        name = state.ToString();
    }

    public Goal(State state, float priority, int timeOut): this(state, priority)
    {
        this.timeOut = timeOut; 
    }

    public override string ToString()
    {
        return state.ToString() + ":" + priority; 
    }


    public List<Goal> Combine(Goal goal)
    {
        List<State> states = this.state.Combine(goal.state);
        if (states.Count == 1) {
            Goal newGoal = new Goal(states[0], this.priority + goal.priority, Mathf.Min(this.timeOut, goal.timeOut));
            newGoal.unlockedActionsOnGoalCompletion.AddRange(unlockedActionsOnGoalCompletion);
            newGoal.unlockedActionsOnGoalCompletion.AddRange(goal.unlockedActionsOnGoalCompletion);

            newGoal.parentGoals.AddRange(parentGoals);
            newGoal.parentGoals.AddRange(goal.parentGoals);

            newGoal.parentGoalObj.AddRange(GetParentGoals());
            newGoal.parentGoalObj.AddRange(goal.GetParentGoals());

            return new List<Goal>() { newGoal };
        } else return new List<Goal>() { this, goal }; 
    }


    public Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new Goal(state.Bind(bindings, resources), priority);
    }

    public List<Goal> GetParentGoals()
    {
        return parentGoalObj;
    }
    public void AddParentGoal(Goal goal)
    {
        if (parentGoalObj.Contains(goal))
            return;

        parentGoalObj.Add(goal);
        parentGoals.Add(goal.name);
    }

    public void AddUnlockedAction(BoundAction action)
    {
        if (unlockedActionsOnGoalCompletion.Contains(action)) return;

        unlockedActionsOnGoalCompletion.Add(action);
    }
}
