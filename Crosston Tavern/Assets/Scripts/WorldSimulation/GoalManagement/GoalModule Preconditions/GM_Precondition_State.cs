public class GM_Precondition_State: GM_Precondition
{
    public State state;
    public bool want;

    public GM_Precondition_State(State state, bool want = true)
    {
        this.state = state;
        this.want = want;
    }

    public override bool Satisfied(WorldState ws)
    {
        return state.InEffect(ws, new BoundBindingCollection(), new FeatureResources()) == want;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        if (want)
            return state.Verbalize(speaker, listener, false);
        else
            return "NOT " + state.Verbalize(speaker, listener, false);
    }
}
