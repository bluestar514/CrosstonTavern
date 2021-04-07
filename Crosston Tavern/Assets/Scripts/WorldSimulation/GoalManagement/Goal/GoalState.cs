using System.Collections.Generic;

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
            GoalState newGoal = new GoalState(states[0], this.priority + goal.priority);
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

