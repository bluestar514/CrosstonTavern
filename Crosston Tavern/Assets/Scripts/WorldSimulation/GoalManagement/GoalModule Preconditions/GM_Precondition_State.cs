using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            return state.Verbalize(speaker, listener, goal: false, futureTense: false);
        else {
            string statement = state.Verbalize(speaker, listener, goal: false, futureTense: false);

            List<string> replace = new List<string>() {
                "am",
                "is",
                "are",
                "have",
                "has",
                "will",
                "can"
            };

            foreach(string pos in replace) {
                statement = statement.Replace(" "+pos+" ", " "+pos+" not ");
            }


            return statement; 
        }
    }

    public override string ToString()
    {
        return "{GoalModule Precondition: State - " +
                        state.ToString() + ":" + want + "}";
    }
}
