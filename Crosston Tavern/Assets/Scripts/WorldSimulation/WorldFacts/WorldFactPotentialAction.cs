using System.Collections.Generic;

[System.Serializable]
public class WorldFactPotentialAction : WorldFact
{
    public BoundAction action;

    public WorldFactPotentialAction(BoundAction action)
    {
        this.action = action;

        id = ToString();
    }

    public override string ToString()
    {
        return "{PotentialAction:" + action.ToString() + "}";
    }


    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactPotentialAction)) return false;

        WorldFactPotentialAction fact = (WorldFactPotentialAction)obj;
        if (this.action.ToString() == fact.ToString()) return true;

        return false;
    }


    public override int GetHashCode()
    {
        return -1387187753 + EqualityComparer<BoundAction>.Default.GetHashCode(action);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);
        return v.VerbalizeAction(action, true);
    }
}
