using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RationalPanel : SubDisplayPanel<WeightedAction.WeightRational>
{
    public Text idText;

    WeightedAction.WeightRational rational;

    public override void Set(WeightedAction.WeightRational rational)
    {
        this.rational = rational;

        Display();
    }

    void Display()
    {
        idText.text = rational.ToString();
    }

}
