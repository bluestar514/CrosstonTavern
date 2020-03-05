using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GoalModule
{

    public List<GM_Precondition> preconditions;
    public List<Goal> goals;

    public GoalModule(List<GM_Precondition> preconditions, List<Goal> goals)
    {
        this.preconditions = preconditions;
        this.goals = goals;
    }

    public virtual bool Precondtion(WorldState ws) {
        if (preconditions.Any(p => !p.Satisfied(ws))) return false;
        else return true;
    }

    

}


public class GM_ManLocation: GoalModule
{
    string location;

    public GM_ManLocation(List<GM_Precondition> preconditions, List<Goal> goals, string locationId) : base(preconditions, goals)
    {
        location = locationId;

        this.goals.Add(new Goal(new Move(location), 10, 1));
    }
}

public class GM_StockFeature: GoalModule
{
    string featureId;

    public GM_StockFeature(List<GM_Precondition> preconditions, List<Goal> goals, string actorId, string featureId, WorldState ws): base(preconditions, goals)
    {
        this.featureId = featureId;

        List<string> stock = ws.map.GetFeature(featureId).relevantResources["stock"];

        foreach (string s in stock) {
            this.goals.Add(new Goal(new InvChange(3, 1000, actorId, new List<string>() { s }), 5, 1));
        }

        this.goals.Add(new Goal(new InvChange(10, 1000, actorId, stock), 3, 1));
    }
}

