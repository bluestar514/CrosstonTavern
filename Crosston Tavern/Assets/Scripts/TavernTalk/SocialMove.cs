using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SocialMove
{
    public string verb;
    public List<string> arguements;

    public List<WorldFact> mentionedFacts;
    public List<WorldFact> retractedFacts;

    public SocialMove(string verb, List<string> arguements = null,
                         List<WorldFact> mentionedFacts = null, List<WorldFact> retractedFacts = null)
    {
        this.verb = verb;

        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;


        if (retractedFacts == null) retractedFacts = new List<WorldFact>();
        this.retractedFacts = retractedFacts;
    }

    public override string ToString()
    {
        string name = verb;
        foreach(string arg in arguements) {
            int index = name.IndexOf("#");
            if(index >0) {
                name = name.Substring(0, index) + 
                        arg + 
                        name.Substring(index+1);
            } 
        }


            return "<" + name + ">";
        
    }
}


[System.Serializable]
public class SocialMenu: SocialMove
{
    public List<SocialMove> menuOptions;
    public SocialMove previousContext;

    public SocialMenu(string verb, List<SocialMove> menuOptions, SocialMove previousContext = null, List<string> arguements = null,
                         List<WorldFact> mentionedFacts = null, List<WorldFact> retractedFacts = null): 
        base(verb, arguements, mentionedFacts, retractedFacts)
    {
        this.menuOptions = menuOptions;

        this.previousContext = previousContext;
        if(previousContext != null) {
            this.menuOptions.Add(previousContext);
        }
        
    }

    public void AddPreviousContext(SocialMove previousContext)
    {
        if(this.previousContext!= null) {
            menuOptions.RemoveAt(menuOptions.Count - 1);
        }

        this.previousContext = previousContext;
        menuOptions.Add(previousContext);
        
    }
}