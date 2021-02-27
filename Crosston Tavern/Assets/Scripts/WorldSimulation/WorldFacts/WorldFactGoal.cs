[System.Serializable]
public class WorldFactGoal: WorldFact
{
    public Goal goal;
    public string owner;

    public WorldFactGoal(Goal goal, string owner)
    {
        this.goal = goal;

        id = ToString();
        this.owner = owner;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is WorldFactGoal)) return false;
        WorldFactGoal other = (WorldFactGoal)obj;

        return goal == other.goal;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "{Goal:"+ owner+ " - " + goal.ToString()+"}";
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);

        if (goal is GoalState goalState) {
            return owner + " wants " + v.VerbalizeState(goalState.state);
        } else if (goal is GoalAction goalAction) {
            return owner + " wants " + v.VerbalizeAction(goalAction.action, true);
        } else return base.Verbalize(speaker, listener, ws);
        
    }
}
