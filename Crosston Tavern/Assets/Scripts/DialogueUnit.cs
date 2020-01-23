[System.Serializable]
public class DialogueUnit
{
    public string verbalization;
    public string speakerName;
    public SocialMove underpinningSocialMove;
    // also probably would hold:
    //  - fact being expressed
    //  - speaker's emotion being displayed
    public DialogueUnit(string verbalization, string speakerName, SocialMove underpinningSocialMove)
    {
        this.verbalization = verbalization;
        this.speakerName = speakerName;
        this.underpinningSocialMove = underpinningSocialMove;
    }

    public override string ToString()
    {
        return "{"+speakerName+"("+underpinningSocialMove+"): \""+verbalization+"\"}";
    }
}