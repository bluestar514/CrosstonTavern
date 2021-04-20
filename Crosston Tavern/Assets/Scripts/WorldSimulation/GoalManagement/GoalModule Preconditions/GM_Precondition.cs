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


public class GM_Precondition_Verbalize: GM_Precondition
{
}

public class GM_Precondition_Verbalize_Preference: GM_Precondition_Verbalize
{
    WorldFactPreference preference;

    public GM_Precondition_Verbalize_Preference(WorldFactPreference preference)
    {
        this.preference = preference;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return preference.Verbalize(speaker, listener, ws);
    }

    public override string ToString()
    {
        return "{GoalModule Precondition Verbalize: Preference - " +
                        preference.ToString() + "}";
    }
}