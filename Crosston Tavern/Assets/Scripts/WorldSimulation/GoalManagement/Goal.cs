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

        name = ToString();
    }

    public Goal(State state, float priority, List<BoundAction> enabledActions):this(state, priority)
    {
        //parentGoals.AddRange(parent);
        enablingActions.AddRange(enabledActions);
    }

    public override string ToString()
    {
        return state.ToString() + ":" + priority; 
    }


    public List<Goal> Combine(Goal goal)
    {
        List<State> states = this.state.Combine(goal.state);
        if (states.Count == 1) {
            Goal newGoal = new Goal(states[0], this.priority + goal.priority);
            newGoal.enablingActions.AddRange(enablingActions);
            newGoal.enablingActions.AddRange(goal.enablingActions);
            return new List<Goal>() { newGoal };
        } else return new List<Goal>() { this, goal }; 
    }


    public Goal MakeSpecific(BoundBindingCollection bindings, FeatureResources resources)
    {
        return new Goal(state.Bind(bindings, resources), priority);
    }
}
