using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RationalPanel : MonoBehaviour
{
    public Text idText;

    WeightedAction.WeightRational rational;

    public void Set(WeightedAction.WeightRational rational)
    {
        this.rational = rational;

        Display();
    }

    void Display()
    {
        idText.text = rational.ToString();
    }

}
