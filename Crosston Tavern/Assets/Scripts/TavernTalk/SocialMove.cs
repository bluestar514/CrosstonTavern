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
    public List<WorldFact> complexFacts;

    public SocialMove(string verb, List<string> arguements = null,
                         List<WorldFact> mentionedFacts = null, 
                         List<WorldFact> retractedFacts = null, 
                         List<WorldFact> complexFacts = null)
    {
        this.verb = verb;

        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;


        if (retractedFacts == null) retractedFacts = new List<WorldFact>();
        this.retractedFacts = retractedFacts;
        if (complexFacts == null) complexFacts = new List<WorldFact>();
        this.complexFacts = complexFacts;
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
