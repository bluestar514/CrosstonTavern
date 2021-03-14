public class GM_Precondition_Time : GM_Precondition
{
    WorldTime startTime;
    WorldTime endTime;

    public GM_Precondition_Time(WorldTime startTime, WorldTime endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public override bool Satisfied(WorldState ws)
    {
        if (startTime < endTime) return ws.Time <= endTime && ws.Time >= startTime;
        else return ws.Time >= startTime || ws.Time <= endTime;
    }

    public override GM_Precondition MakeSpecific(WorldState ws)
    {
        return base.MakeSpecific(ws);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        return "It was the right time";
    }

}
