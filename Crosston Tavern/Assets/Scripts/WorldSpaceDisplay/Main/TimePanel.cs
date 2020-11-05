using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimePanel : MonoBehaviour
{
    public int timeStep;

    public WorldTime timeStamp;

    public Text display;

    public void Set(int time, WorldTime timeStamp)
    {
        timeStep = time;
        this.timeStamp = timeStamp;

        display.text = timeStep.ToString() +":"+timeStamp.ToString() ;
    }
}
