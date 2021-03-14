public class GM_Precondition_State: GM_Precondition
{
    public State state;

    public GM_Precondition_State(State state)
    {
        this.state = state;
    }

    public override bool Satisfied(WorldState ws)
    {
        return state.InEffect(ws, new BoundBindingCollection(), new FeatureResources());
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return state.Verbalize(speaker, listener, false);
    }
}
