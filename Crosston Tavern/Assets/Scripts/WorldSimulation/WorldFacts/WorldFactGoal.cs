using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldFactGoal: WorldFact
{
    public Goal goal;
    public string owner;
    public List<Modifier> modifier = new List<Modifier>();

    public enum Modifier
    {
        player,
        stuck,
        highPriority,
        error
    }

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

        return goal.Equals(other.goal);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "{Goal:"+ owner+ " - " + goal.ToString()+ "["+ string.Join(",",modifier) + "]}";
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);

        string o = VerbalizationDictionary.Replace(owner);


        return o + " wants " + v.VerbalizeGoal(goal);
    }
}
