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

    public SocialMove(string verb, List<string> arguements = null, string content = "")
    {
        this.verb = verb;
        this.content = content;
        if (arguements == null) arguements = new List<string>();
        this.arguements = arguements;
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
