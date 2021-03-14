using System.Collections;
using UnityEngine;

public class GM_Precondition
{
    public virtual bool Satisfied(WorldState ws)
    {
        return true;
    }

    public virtual GM_Precondition MakeSpecific(WorldState ws)
    {
        return this;
    }

    public virtual string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "ERROR";
    }
}
