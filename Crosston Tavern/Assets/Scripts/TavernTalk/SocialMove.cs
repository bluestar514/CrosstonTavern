using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//For the moment they are just fancy strings, 
//but eventually we can imagine they have more 
//complicated structure holding far more information
//maybe even information on how to be displayed

[System.Serializable]
public class SocialMove
{
    public string verb;
    public List<string> arguements;
    public string content;

    public List<WorldFact> mentionedFacts;

    public SocialMove(string verb, List<string> arguements = null, string content = "",
        List<ExecutedAction> mentionedActions = null, List<Goal> mentionedGoals = null, List<WorldFact> mentionedFacts = null)
    {
        this.verb = verb;

        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;
        if (mentionedActions != null) {
            foreach (ExecutedAction action in mentionedActions) {
                this.mentionedFacts.Add(new WorldFactEvent(action));
            }
        }
        if (mentionedGoals != null) {
            foreach (Goal goal in mentionedGoals) {
                this.mentionedFacts.Add(new WorldFactGoal(goal));
            }
        }


        this.content = content + string.Join(",", mentionedFacts);
        
    }

    public override string ToString()
    {
        string name = verb;
        foreach(string arg in arguements) {
            name = name.Replace("#", arg);
        }

        if (content == "")
            return "<" + name + ">";
        else
            return "<" + name + ":" + content + ">";
    }
}
