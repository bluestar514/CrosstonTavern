using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM_Precondition_PlayerInstructed : GM_Precondition
{
    public string director;
    public string directed;

    public GM_Precondition_PlayerInstructed(string director, string directed)
    {
        this.director = director;
        this.directed = directed;
    }

    public override bool Satisfied(WorldState ws)
    {
        return true;
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string director = this.director;
        director = Verbalizer.VerbalizeSubject(director, speaker, listener);
        string directed = this.directed;
        directed = Verbalizer.VerbalizeSubject(directed, speaker, listener);
        if (directed == "I") directed = "me";

        return director + " told " + directed + " to.";

    }

    public override string ToString()
    {
        return "{GoalModule Precondition: Directed - (" +
                        director + "->" +  directed + ")}";
    }
}
