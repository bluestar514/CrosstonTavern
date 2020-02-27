
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PreconditionsDisplayPanel : SubDisplayPanel<Condition>
{
    public Text idText;

    Condition condition;

    public override void Set(Condition condition)
    {
        this.condition = condition;

        Display();
    }

    void Display()
    {
        idText.text = condition.ToString();

    }

}