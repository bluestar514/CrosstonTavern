using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutedAction : ChosenAction
{
    public List<Effect> executedEffect;
    public WorldTime executionTime;

    public ExecutedAction(WeightedAction chosenAction, List<BoundAction> invalidChoices, List<WeightedAction> rejectedChoices, List<Effect> executedEffect, WorldTime executionTime) : base(chosenAction, invalidChoices, rejectedChoices)
    {
        this.executedEffect = executedEffect;
        this.executionTime = executionTime;
    }

    public ExecutedAction(ChosenAction chosenAction, List<Effect> executedEffect, WorldTime executionTime) : this(chosenAction.Action, chosenAction.invalidChoices, chosenAction.rejectedChoices, executedEffect, executionTime) {
        inProgress = chosenAction.inProgress;
    }

    public override string ToString()
    {
        return "{" + Action + "-" + executionTime + "}";
    }

}
