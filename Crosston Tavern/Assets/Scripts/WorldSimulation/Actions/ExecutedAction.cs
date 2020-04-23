using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutedAction : ChosenAction
{
    public Outcome executedEffect;

    public ExecutedAction(WeightedAction chosenAction, List<BoundAction> invalidChoices, List<WeightedAction> rejectedChoices, Outcome executedEffect) : base(chosenAction, invalidChoices, rejectedChoices)
    {
        this.executedEffect = executedEffect;
    }

    public ExecutedAction(ChosenAction chosenAction, Outcome executedEffect) : this(chosenAction.Action, chosenAction.invalidChoices, chosenAction.rejectedChoices, executedEffect) {
        inProgress = chosenAction.inProgress;
    }

}
