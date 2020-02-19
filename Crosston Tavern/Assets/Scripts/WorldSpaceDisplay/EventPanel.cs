using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    public string townieRow;
    public int timeStep;
    public ExecutedAction action;

    public Text display;

    public void Set(string townie, ExecutedAction action, int time)
    {
        townieRow = townie;
        timeStep = time;
        this.action = action;


        display.text = action.ToString();
    }
}
