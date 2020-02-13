using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
    public int timeStep;

    public Text display;

    public void Set(int time)
    {
        timeStep = time;

        display.text = timeStep.ToString();
    }
}
