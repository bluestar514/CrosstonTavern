using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMoveTellAboutGoal : SocialMove
{
    public SocialMoveTellAboutGoal( Townie speaker,
        List<string> arguements = null, 
        string content = "") : 
        base(arguements, content, null)
    {
        FillFacts(speaker);
    }

    protected override void FillFacts(Townie speaker)
    {
        mentionedFacts = MakeGoalsFacts(speaker, speaker.gm.GetGoalsList());
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        Verbalizer v = new Verbalizer(speaker, listener, ws);

        string verbilization;

        if (mentionedFacts.Count == 0) {
            verbilization = "I just do.";
            return verbilization;
        }

        List<string> motivations = new List<string>();
        List<string> goals = new List<string>();

        //I want to have 3 to 1000 trout
        //I want to be friendlier with Alicia
        //I want to be dating Alicia
        //I want Alicia to have 4 to 5 strawberry
        foreach (WorldFact fact in mentionedFacts) {
            if (fact is WorldFactPotentialAction) {
                WorldFactPotentialAction actionFact = (WorldFactPotentialAction)fact;
                motivations.Add(v.VerbalizeAction(actionFact.action, true));
            }
            if (fact is WorldFactGoal) {
                WorldFactGoal goalFact = (WorldFactGoal)fact;
                goals.Add(v.VerbalizaeState(goalFact.goal.state));
            }
        }
        verbilization = "";
        if (motivations.Count > 0) verbilization = "I want to " + string.Join(". I want to ", motivations) + ". ";
        verbilization = verbilization + "I want " + string.Join(". I want ", goals) + ". ";

        return verbilization;
    }

}


public class SocialMoveTellAboutDayEvents : SocialMove
{
    public SocialMoveTellAboutDayEvents(Townie speaker, List<string> arguements = null, 
        string content = "") : 
        base(arguements, content, null)
    {
        FillFacts(speaker);
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string verbilization;

        float coin = Random.value;
        if (coin > .5)
            verbilization = "Today, " + VerbalizeAllEvents(mentionedFacts);
        else
            verbilization = VerbalizeByTimePeriod(mentionedFacts);

        return verbilization;
    }

    protected override void FillFacts(Townie speaker)
    {
        List<ExecutedAction> history = GetDayEvents(speaker);
        history = FilterMyActions(speaker, history);

        mentionedFacts = MakeActionFacts(history);
    }
}