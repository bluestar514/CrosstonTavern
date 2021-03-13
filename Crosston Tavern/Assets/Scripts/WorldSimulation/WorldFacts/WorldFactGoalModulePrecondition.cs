using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class WorldFactGoalModulePrecondition : WorldFact
{
    public GM_Precondition precondition;

    public WorldFactGoalModulePrecondition(GM_Precondition precondition)
    {
        this.precondition = precondition;
    }


    public override bool Equals(object obj)
    {
        if(obj is WorldFactGoalModulePrecondition wf) {
            return precondition == wf.precondition;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return 1350471067 + EqualityComparer<GM_Precondition>.Default.GetHashCode(precondition);
    }

    public override string ToString()
    {
        return precondition.ToString();
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return precondition.Verbalize(speaker, listener, ws);
    }
}
