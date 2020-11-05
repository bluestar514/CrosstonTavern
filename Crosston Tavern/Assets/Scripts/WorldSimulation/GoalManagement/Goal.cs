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
    public List<BoundAction> enablingActions = new List<BoundAction>(); // THese are the actions completing this goal unlocks
    //Not the actions which complete this goal

    public Goal(State state, float priority)
    {
        this.state = state;
        this.priority = priority;

        name = ToString();
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
            newGoal.enablingActions.AddRange(enablingActions);
            newGoal.enablingActions.AddRange(goal.enablingActions);
            newGoal.parentGoals.AddRange(parentGoals);
            newGoal.parentGoals.AddRange(goal.parentGoals);
            return new List<Goal>() { newGoal };
        } else return new List<Goal>() { this, goal }; 
    }


    public Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new Goal(state.Bind(bindings, resources), priority);
    }
}
