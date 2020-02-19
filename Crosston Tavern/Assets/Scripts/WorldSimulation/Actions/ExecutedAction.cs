using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutedAction : ChosenAction
{
    public Effect executedEffect;

    public ExecutedAction(WeightedAction chosenAction, List<WeightedAction> rejectedChoices, Effect executedEffect) : base(chosenAction, rejectedChoices)
    {
        this.executedEffect = executedEffect;
    }

    public ExecutedAction(ChosenAction chosenAction, Effect executedEffect) : this(chosenAction.Action, chosenAction.rejectedChoices, executedEffect) { }

}
