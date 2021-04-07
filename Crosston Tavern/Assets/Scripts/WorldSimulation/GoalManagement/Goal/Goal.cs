using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Goal
{
    public string name;
    public float priority;

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
        return new List<Goal> (parentGoalObj);
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

