using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueUnit
{
    public string verbalization;
    public string speakerName;
    public SocialMove underpinningSocialMove;

    public NPCPortrait.State emotion;

    public DialogueUnit(string verbalization, string speakerName, SocialMove underpinningSocialMove, NPCPortrait.State emotion)
    {
        this.verbalization = verbalization;
        this.speakerName = speakerName;
        this.underpinningSocialMove = underpinningSocialMove;
        this.emotion = emotion;
    }

    public override string ToString()
    {
        return "{"+speakerName+"("+underpinningSocialMove+"): \""+verbalization+"\"}";
    }
}

public class CompoundDialogueUnit : DialogueUnit
{
    public List<DialogueUnit> components;

    public CompoundDialogueUnit(List<DialogueUnit> components): 
        base(string.Join("\n", components),"CompoundDialogueUnit", null, NPCPortrait.State.none)
    {
        this.components = components;
    }

    public override string ToString()
    {
        return "[" +string.Join(",", components)+ "]";
    }
}