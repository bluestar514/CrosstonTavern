using System.Collections.Generic;
using System.Linq;

public class GM_Precondition_TopRelationAxis: GM_Precondition
{
    public string owner;
    public string target; 
    public Relationship.Axis axis;
    public int top;
    public bool reverse; //if true, we want to be in the bottom # instead
    public bool want; // whether we want to be in the top # or if we expilicitly don't

    public GM_Precondition_TopRelationAxis(Relationship.Axis axis,  string owner, string target, 
                                            int top = 1, bool reverse = false, bool want = true)
    {
        this.axis = axis;
        this.top = top;
        this.want = want;
        this.owner = owner;
        this.target = target;
        this.reverse = reverse;
    }

    public override bool Satisfied(WorldState ws)
    {
        Relationship rel = ws.GetRelationshipsFor(owner);


        List<KeyValuePair<string, int>> pairs = new List<KeyValuePair<string, int>>();
        foreach (string knownPerson in rel.GetKnownPeople()) {
            pairs.Add(new KeyValuePair<string, int>(knownPerson, rel.Get(knownPerson, axis)));
        }
        if (reverse) {
            pairs = pairs.OrderBy(pair => pair.Value).ToList();
        } else {
            pairs = pairs.OrderBy(pair => -pair.Value).ToList();
        }
        

        int position = pairs.FindIndex(pair => pair.Key == target);


        return (position < top) == want;
    }

    public override string ToString()
    {
        if (reverse) {
            return "{GoalModule Precondition: TopRelationAxis - " + 
                        owner + "-" + axis + "->" + target + 
                        " (Bottom " + top + ":"+want+ ")}";
        } else {
            return "{GoalModule Precondition: TopRelationAxis - " + 
                        owner + "-" + axis + "->" + target + 
                        " (Top " + top + ":" + want + ")}";
        }
        
    }

    public override string Verbalize(string speaker, string listener, WorldState ws = null)
    {
        string owner = Verbalizer.VerbalizeSubject(this.owner, speaker, listener);
        string target = Verbalizer.VerbalizeSubject(this.target, speaker, listener);

        string others = "more than anyone";
        if (top > 1) others = "more than almost anyone else";

        string opinion = "";
        if(axis == Relationship.Axis.friendly) {
            if (reverse) {
                opinion = "dislike";
            } else {
                opinion = "like";
            }
        } else {
            if (reverse) {
                opinion = "hate";
            } else {
                opinion = "love";
            }
        }


        return string.Join(" ", new List<string>() { owner, opinion, target, others });
    }
}