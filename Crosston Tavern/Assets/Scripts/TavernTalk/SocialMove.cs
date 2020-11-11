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

    public List<ExecutedAction> mentionedActions;
    public List<Goal> mentionedGoals;

    public SocialMove(string verb, List<string> arguements = null, string content = "", 
        List<ExecutedAction> mentionedActions = null, List<Goal> mentionedGoals = null)
    {
        this.verb = verb;
        
        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;
        if (mentionedActions == null) mentionedActions = new List<ExecutedAction>();
        this.mentionedActions = mentionedActions;
        if (mentionedGoals == null) mentionedGoals = new List<Goal>();
        this.mentionedGoals = mentionedGoals;


        this.content = content + string.Join(",", mentionedGoals)+string.Join(",", mentionedActions);
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
