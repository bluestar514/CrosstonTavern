using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChosenAction
{
    [SerializeField]
    string name;
    [SerializeField]
    WeightedAction chosenAction;
    public WeightedAction Action { get => chosenAction; private set => chosenAction = value; }

    [SerializeField]
    List<WeightedAction> rejectedChoices;

    public ChosenAction(WeightedAction chosenAction, List<WeightedAction> rejectedChoices)
    {
        this.chosenAction = chosenAction;
        this.rejectedChoices = rejectedChoices;

        name = ToString();
    }

    public override string ToString()
    {
        return chosenAction.ToString();
    }
}
