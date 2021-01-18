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

    public SocialMove(string verb, List<string> arguements = null, string content = "",
                         List<WorldFact> mentionedFacts = null)
    {
        this.verb = verb;

        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;

        if (mentionedFacts == null) mentionedFacts = new List<WorldFact>();
        this.mentionedFacts = mentionedFacts;

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
