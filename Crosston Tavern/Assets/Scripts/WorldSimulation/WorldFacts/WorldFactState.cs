using System.Collections.Generic;

class WorldFactState: WorldFact
{
    public State state;

    public WorldFactState(State state)
    {
        this.state = state;
    }

    public override WorldFact Bind(BoundBindingCollection bindings, FeatureResources resources)
    {
        return base.Bind(bindings, resources);
    }

    public override bool Equals(object obj)
    {
        return state.Equals(obj);
    }

    public override int GetHashCode()
    {
        return 259708774 + EqualityComparer<State>.Default.GetHashCode(state);
    }

    public override string ToString()
    {
        return "{State: " + state.ToString() + "}";
    }

    public override List<WorldFact> UpdateWorldState(WorldState ws)
    {
        return base.UpdateWorldState(ws);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return base.Verbalize(speaker, listener, ws);
    }
}