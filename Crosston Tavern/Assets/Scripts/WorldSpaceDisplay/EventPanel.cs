using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    public Townie townieRow;
    public int timeStep;
    public WorldAction action;

    public Text display;

    public void Set(Townie townie, WorldAction action, int time)
    {
        townieRow = townie;
        timeStep = time;
        this.action = action;


        display.text = action.ToString();
    }
}
