﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExecutedAction : ChosenAction
{
    public List<Effect> executedEffect;
    public WorldTime executionTime;

    public Opinion opinion;


    public ExecutedAction(WeightedAction chosenAction, List<BoundAction> invalidChoices, 
                            List<WeightedAction> rejectedChoices, List<Effect> executedEffect,
                            WorldTime executionTime) : base(chosenAction, invalidChoices, rejectedChoices)
    {
        this.executedEffect = executedEffect;
        this.executionTime = executionTime;

        this.opinion = new Opinion();
    }

    public ExecutedAction(ChosenAction chosenAction, 
                            List<Effect> executedEffect, 
                            WorldTime executionTime) : 
                            this(chosenAction.Action, chosenAction.invalidChoices, 
                                chosenAction.rejectedChoices, executedEffect, 
                                executionTime) {
        inProgress = chosenAction.inProgress;
    }

    /// <summary>
    /// ONLY THE OPINION RELATED FEILDS ARE INDEPENDANT OBJECTS!!!
    /// All others are shared between copy and original.
    /// We actually don't even bother copying the opinion fields.
    /// </summary>
    /// <returns></returns>
    public ExecutedAction ShallowCopy() 
    {
        return new ExecutedAction(Action, invalidChoices, rejectedChoices, executedEffect, executionTime);
    }

    public override string ToString()
    {
        return "{" + Action + "-" + executionTime + "}";
    }

}
