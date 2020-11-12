using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueUnit
{
    public string verbalization;
    public string speakerName;
    public SocialMove underpinningSocialMove;
    public List<WorldFact> facts = new List<WorldFact>();
    // also probably would hold:
    //  - speaker's emotion being displayed
    
    public DialogueUnit(string verbalization, string speakerName, SocialMove underpinningSocialMove)
    {
        this.verbalization = underpinningSocialMove.ToString();
        this.speakerName = speakerName;
        this.underpinningSocialMove = underpinningSocialMove;
        SocialMoveContentToFacts(underpinningSocialMove);
    }

    void SocialMoveContentToFacts(SocialMove socialMove)
    {
        foreach(WorldFact fact in socialMove.mentionedFacts) {
            facts.Add(fact);
        }

    }

    public override string ToString()
    {
        return "{"+speakerName+"("+underpinningSocialMove+"): \""+verbalization+"\"}";
    }
}

