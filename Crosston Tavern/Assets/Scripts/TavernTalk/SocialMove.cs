using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SocialMove
{
    public string verb;
    public List<string> arguements;
    public string content;

    public List<WorldFact> mentionedFacts;
    public List<WorldFact> retractedFacts;

    public SocialMove(string verb, List<string> arguements = null, string content = "",
                         List<WorldFact> mentionedFacts = null, List<WorldFact> retractedFacts = null)
    {
        this.verb = verb;

        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;

        this.content = content + string.Join(",", mentionedFacts);

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

        if (content == "")
            return "<" + name + ">";
        else
            return "<" + name + ":" + content + ">";
    }
}
