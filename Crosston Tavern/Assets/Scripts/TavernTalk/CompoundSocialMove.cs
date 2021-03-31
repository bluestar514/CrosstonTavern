using System.Collections.Generic;

public class CompoundSocialMove: SocialMove
{
    public List<SocialMove> socialMoves;

    public CompoundSocialMove(string verb, List<string> arguements = null,
                                           List<WorldFact> mentionedFacts = null,
                                           List<WorldFact> retractedFacts = null,
                                           List<SocialMove> socialMoves = null) :
        base(verb, arguements, mentionedFacts, retractedFacts)
    {
        if (socialMoves == null) socialMoves = new List<SocialMove>();
        this.socialMoves = socialMoves;

        foreach(SocialMove move in socialMoves) {
            this.mentionedFacts.AddRange(move.mentionedFacts);
            this.retractedFacts.AddRange(move.retractedFacts);
        }
    }

    public override string ToString()
    {
        string name = verb;
        foreach (string arg in arguements) {
            int index = name.IndexOf("#");
            if (index > 0) {
                name = name.Substring(0, index) +
                        arg +
                        name.Substring(index + 1);
            }
        }


        return "<" + name+ ":{"+ string.Join(",", socialMoves) + "}>";
    }
}