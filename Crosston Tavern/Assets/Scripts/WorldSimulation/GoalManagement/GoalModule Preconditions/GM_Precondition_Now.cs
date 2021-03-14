public class GM_Precondition_Now: GM_Precondition
{
    public WorldTime length;

    public GM_Precondition_Now(WorldTime length)
    {
        this.length = length;
    }

    public override bool Satisfied(WorldState ws)
    {
        return MakeSpecific(ws).Satisfied(ws);
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        WorldTime start = new WorldTime(ws.Time);

        WorldTime end = start + length;

        return new GM_Precondition_Time(start, end);
    }
    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "It was the right time";
    }
}
