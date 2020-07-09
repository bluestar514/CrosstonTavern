using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnownEvent : MonoBehaviour
{
    public ExecutedAction executedAction;
    public Text displayText;


    public void Setup(ExecutedAction executedAction)
    {
        this.executedAction = executedAction;

        displayText.text = executedAction.ToString();
    }
}
