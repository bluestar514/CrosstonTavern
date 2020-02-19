using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDetailPanel : DetailTab
{
    public ChosenAction action;

    public void Set(ChosenAction action)
    {
        this.action = action;

        displayName.text = action.ToString();
        //location.text = action.location;


        //body.text = action.StringifyStats();
    }

}
