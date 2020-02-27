using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutedAction : ChosenAction
{
    public Effect executedEffect;

    public ExecutedAction(WeightedAction chosenAction, List<BoundAction> invalidChoices, List<WeightedAction> rejectedChoices, Effect executedEffect) : base(chosenAction, invalidChoices, rejectedChoices)
    {
        this.executedEffect = executedEffect;
    }

    public ExecutedAction(ChosenAction chosenAction, Effect executedEffect) : this(chosenAction.Action, chosenAction.invalidChoices, chosenAction.rejectedChoices, executedEffect) {
        inProgress = chosenAction.inProgress;
    }

}
