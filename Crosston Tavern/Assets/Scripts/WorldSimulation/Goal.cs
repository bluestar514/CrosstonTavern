using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal 
{
    public float priority;
    public MicroEffect goalState;

    public Goal(float priority, MicroEffect goalState)
    {
        this.priority = priority;
        this.goalState = goalState;
    }
}
