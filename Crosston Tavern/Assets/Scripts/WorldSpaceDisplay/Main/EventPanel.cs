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

    public EventDetailPanel eventDetailPanel;

    public void Set(string townie, ExecutedAction action, int time, EventDetailPanel eventDetailPanel)
    {
        townieRow = townie;
        timeStep = time;
        this.action = action;

        this.eventDetailPanel = eventDetailPanel;

        display.text = action.ToString();
    }

    public void OpenEventDetailsPanel()
    {
        eventDetailPanel.Set(action);
        eventDetailPanel.OpenTab();
    }
}
