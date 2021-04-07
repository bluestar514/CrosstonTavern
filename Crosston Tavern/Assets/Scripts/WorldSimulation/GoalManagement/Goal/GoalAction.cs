using System.Collections.Generic;

[System.Serializable]
public class GoalAction : Goal
{
    public BoundAction action;

    public GoalAction(BoundAction action, float priority)
    {
        this.action = action;
        this.priority = priority;

        name = action.ToString();
    }

    public override List<Goal> Combine(Goal goal)
    {
        if (goal is GoalAction goalAction) {
            BoundAction otherAction = goalAction.action;

            if (action == otherAction) {
                Goal newGoal = new GoalAction(action, this.priority + goal.priority);
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

