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

    public List<BoundAction> invalidChoices;
    public List<WeightedAction> rejectedChoices;
    

    public int inProgress = 0;

    public ChosenAction(WeightedAction chosenAction, List<BoundAction> invalidChoices, List<WeightedAction> rejectedChoices)
    {
        this.chosenAction = chosenAction;
        this.invalidChoices = invalidChoices;
        this.rejectedChoices = rejectedChoices;

        name = ToString();
    }

    public override string ToString()
    {
        return "<" + chosenAction.ToString();//"("+inProgress+"/"+chosenAction.executionTime+")>";
    }

    public string VerboseString()
    {
        string str = ToString() +
            "\nRationals:\n\t"+ string.Join("\n\t", chosenAction.weightRationals)+
            "\nInvalid Actions: \n\t" + string.Join("\n\t", invalidChoices)+
            "\nRejected Actions:\n\t" + string.Join("\n\t", rejectedChoices);

        return chosenAction.Bindings.BindString(str);
    }

    public void Progress()
    {
        inProgress++;
    }
    public bool Complete()
    {
        return inProgress >= Action.executionTime;
    }
    public bool Started()
    {
        return inProgress > 0;
    }
}
