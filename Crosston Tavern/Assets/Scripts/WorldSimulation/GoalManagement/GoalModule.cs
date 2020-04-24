using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GoalModule
{
    public string name;


    public List<GM_Precondition> preconditions;
    public List<Goal> goals;

    public GoalModule(List<GM_Precondition> preconditions, List<Goal> goals)
    {
        this.preconditions = preconditions;
        this.goals = goals;

        name = "GENERIC GOAL";
    }

    public virtual bool Precondtion(WorldState ws) {
        if (preconditions.Any(p => !p.Satisfied(ws))) return false;
        else return true;
    }

    

}

[System.Serializable]
public class GM_ManLocation: GoalModule
{
    string location;

    public GM_ManLocation(List<GM_Precondition> preconditions, List<Goal> goals, string locationId) : base(preconditions, goals)
    {
        location = locationId;

        this.goals.Add(new Goal(new EffectMove(location), 10, 1));

        name = "Man " + location;
    }
}

[System.Serializable]
public class GM_StockFeature: GoalModule
{
    string featureId;

    public GM_StockFeature(List<GM_Precondition> preconditions, List<Goal> goals, string actorId, string featureId, WorldState ws): base(preconditions, goals)
    {
        this.featureId = featureId;

        List<string> stock = ws.map.GetFeature(featureId).relevantResources["stock"];

        foreach (string s in stock) {
            this.goals.Add(new Goal(new EffectInvChange(new NumericValue<int>(3, 1000), actorId, new List<string>() { s }), 5, 1));
        }

        this.goals.Add(new Goal(new EffectInvChange(new NumericValue<int>(10, 1000), actorId, stock), 3, 1));


        name = "Stock " + featureId;
    }
}

