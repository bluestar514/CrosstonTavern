using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : Effect
{

    public virtual List<Effect> MakeActionable(WorldState ws, Person actor)
    {
        return new List<Effect>() { this };
    }
}
