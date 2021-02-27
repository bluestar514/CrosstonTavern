using System.Collections.Generic;

[System.Serializable]
public class WorldFactEvent: WorldFact
{
    public ExecutedAction action;

    public WorldFactEvent(ExecutedAction action)
    {
        this.action = action;

        id = ToString();
    }

    public override string ToString()
    {
        return "{Event:" + action.ToString() + "}";
    }


    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactEvent)) return false;

        WorldFactEvent fact = (WorldFactEvent)obj;
        if (this.action.ToString() == fact.action.ToString()) return true;

        return false;
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        List<WorldFact> learnedFacts = ws.AddHistory(action);
        learnedFacts.Add(this);

        return learnedFacts;
    }

    public override int GetHashCode()
    {
        return -1387187753 + EqualityComparer<ExecutedAction>.Default.GetHashCode(action);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);
        return v.VerbalizeAction(action, false);
    }
}
